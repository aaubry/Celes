using Celes.Common;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Transactions;

namespace Celes.EntityFramework
{
	public class DbContextContentPathCache : IContentPathCache
	{
		private readonly Type _rootContentType;
		private readonly DbContext _dbContext;

		public DbContextContentPathCache(DbContext dbContext, Type rootContentType)
		{
			if (dbContext == null)
			{
			    throw new ArgumentNullException("dbContext");
			}

			if (rootContentType == null)
			{
				throw new ArgumentNullException("rootContentType");
			}

			_dbContext = dbContext;
			_rootContentType = rootContentType;
		}
	
		private class ContentPathCacheEntryAdapter : IContentPathCacheEntry
		{
			public int ContentId { get; set; }
			public ContentPath Path { get; set; }
			public Type ContentType { get; set; }
			public bool HasChildren { get; set; }
		}

		private IContentPathCacheEntry ParseCacheEntry(ContentPathCacheEntryDto entry)
		{
			return new ContentPathCacheEntryAdapter
			{
				ContentId = entry.ContentId,
				ContentType = LoadType(entry.ContentType),
				Path = ContentPath.Parse(entry.Path),
				HasChildren = entry.HasChildren,
			};
		}

		protected virtual Type LoadType(string typeName)
		{
			var type = _dbContext.GetType().Assembly.GetType(typeName, throwOnError: false);
			if (type != null)
			{
				return type;
			}

			return Type.GetType(typeName, throwOnError: true);
		}

		private DbSet<ContentPathCacheEntry> ContentPathCacheEntries { get { return _dbContext.Set<ContentPathCacheEntry>(); } }

		#region IContentPathCache Members

		IEnumerable<IContentPathCacheEntry> IContentPathCache.GetAllEntries()
		{
			EnsureCacheIsBuilt();

			return ContentPathCacheEntries
				.ToContentPathCacheEntryDto()
				.AsEnumerable()
				.Select(ParseCacheEntry);
		}

		IContentPathCacheEntry IContentPathCache.GetEntryByPath(ContentPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			
			EnsureCacheIsBuilt();

			var cacheEntry = GetEntryByPathQuery(path)
				.ToContentPathCacheEntryDto()
				.FirstOrDefault();

			if (cacheEntry == null)
			{
				throw new ContentNotFoundException(path);
			}

			return ParseCacheEntry(cacheEntry);
		}

		void IContentPathCache.Invalidate()
		{
			var options = new TransactionOptions { IsolationLevel = IsolationLevel.Serializable };
			using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, options))
			{
				_dbContext.Database.ExecuteSqlCommand(string.Format("delete from [{0}]", ContentPathCacheEntry.TableName));

				// Only a single root is allowed
				var roots = ((IEnumerable)_dbContext.Set(_rootContentType)).Cast<IContent>().Take(1);
				RebuildPathCache(roots, null, null);

				_dbContext.SaveChanges();

				transaction.Complete();
			}
		}

		void IContentPathCache.UpdatePath(ContentPath oldPath, ContentPath newPath)
		{
			if (oldPath == null)
			{
				throw new ArgumentNullException("oldPath");
			}

			if (newPath == null)
			{
				throw new ArgumentNullException("newPath");
			}

			var cacheEntry = GetEntryByPath(oldPath);
			UpdatePathRecursive(oldPath.ToString(), newPath.ToString(), cacheEntry);
			_dbContext.SaveChanges();
		}

		private void UpdatePathRecursive(string oldPathPrefix, string newPathPrefix, ContentPathCacheEntry cacheEntry)
		{
			if (!cacheEntry.Path.StartsWith(oldPathPrefix))
			{
				throw new ArgumentException(string.Format("Cache entry path '{0}' does not start with '{1}'.", cacheEntry.Path, oldPathPrefix));
			}

			var newPath = newPathPrefix + cacheEntry.Path.Substring(oldPathPrefix.Length);

			SetPath(cacheEntry, newPath);

			foreach (var childCacheEntry in cacheEntry.Children)
			{
				UpdatePathRecursive(oldPathPrefix, newPathPrefix, childCacheEntry);
			}
		}

		void IContentPathCache.RemovePath(ContentPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}

			var cacheEntry = GetEntryByPath(path);
			RemoveCacheEntryTree(cacheEntry);
			_dbContext.SaveChanges();
		}

		void IContentPathCache.AddPath(ContentPath path, int contentId, Type contentType)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}

			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			
			var parentEntry = path != ContentPath.Root
				? GetEntryByPath(path.GetParent())
				: null;

			var sortOrder = parentEntry != null && parentEntry.Children.Any()
				? parentEntry.Children.Max(e => e.SortOrder) + 1
				: 0;

			var cacheEntry = CreateCacheEntry(contentId, contentType, path, parentEntry, sortOrder);
			_dbContext.SaveChanges();
		}

		IEnumerable<IContentPathCacheEntry> IContentPathCache.GetChildEntriesOfPath(ContentPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			
			EnsureCacheIsBuilt();

			return GetEntryByPathQuery(path)
				.SelectMany(e => e.Children)
				.OrderBy(e => e.SortOrder)
				.ToContentPathCacheEntryDto()
				.AsEnumerable()
				.Select(ParseCacheEntry);
		}

		bool IContentPathCache.ReorderPath(ContentPath path, bool moveUp)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			
			if (path == ContentPath.Root)
			{
				return false;
			}

			EnsureCacheIsBuilt();

			var entryToMove = GetEntryByPath(path);

			ContentPathCacheEntry siblingEntry;
			if (moveUp)
			{
				siblingEntry = GetEntryByPathQuery(path.GetParent())
					.SelectMany(p => p.Children)
					.Where(p => p.SortOrder < entryToMove.SortOrder)
					.OrderByDescending(p => p.SortOrder)
					.FirstOrDefault();
			}
			else
			{
				siblingEntry = GetEntryByPathQuery(path.GetParent())
					.SelectMany(p => p.Children)
					.Where(p => p.SortOrder > entryToMove.SortOrder)
					.OrderBy(p => p.SortOrder)
					.FirstOrDefault();
			}

			if (siblingEntry != null)
			{
				var tmp = entryToMove.SortOrder;
				entryToMove.SortOrder = siblingEntry.SortOrder;
				siblingEntry.SortOrder = tmp;

				_dbContext.SaveChanges();
				return true;
			}

			return false;
		}

		#endregion

		private static readonly object _cacheInitializationMutex = new object();
		private static bool _cacheInitialized = false;

		private void EnsureCacheIsBuilt()
		{
			lock (_cacheInitializationMutex)
			{
				if (!_cacheInitialized)
				{
					if (!ContentPathCacheEntries.Any())
					{
						((IContentPathCache)this).Invalidate();
					}
					_cacheInitialized = true;
				}
			}
		}

		private void RemoveCacheEntryTree(ContentPathCacheEntry cacheEntry)
		{
			foreach (var childEntry in cacheEntry.Children.ToList())
			{
				RemoveCacheEntryTree(childEntry);
			}

			ContentPathCacheEntries.Remove(cacheEntry);
		}

		private ContentPathCacheEntry GetEntryByPath(ContentPath path)
		{
			var pathCacheEntryQuery = GetEntryByPathQuery(path);
			var cacheEntry = pathCacheEntryQuery.FirstOrDefault();
			if (cacheEntry == null)
			{
				throw new ContentNotFoundException(path);
			}

			return cacheEntry;
		}

		private IQueryable<ContentPathCacheEntry> GetEntryByPathQuery(ContentPath path)
		{
			var pathString = path.ToString();
			var pathHash = KnuthHash.CalculateHash(pathString);

			var pathCacheEntryQuery = ContentPathCacheEntries
				.Where(e => e.Hash == pathHash && e.Path == pathString);
			return pathCacheEntryQuery;
		}

		private static readonly ConcurrentDictionary<Type, ICollection<PropertyInfo>> _collectionPropertiesByType
			= new ConcurrentDictionary<Type, ICollection<PropertyInfo>>();

		private void RebuildPathCache(IEnumerable<IContent> contents, ContentPath parentPath, ContentPathCacheEntry parentEntry)
		{
			var cachedContents = contents.ToList();
			if (cachedContents.Count == 0)
			{
				return;
			}

			foreach (var item in cachedContents.Select((c, i) => new { Content = c, Index = i }))
			{
				var content = item.Content;

				var path = parentPath != null
					? parentPath.Append(content.Alias)
					: ContentPath.Root;

				var actualContentType = ObjectContext.GetObjectType(content.GetType());

				var collectionProperties = _collectionPropertiesByType.GetOrAdd(actualContentType, t => t
					.GetProperties(BindingFlags.Public | BindingFlags.Instance)
					.Select(p => new { Property = p, CollectionType = p.PropertyType.GetImplementationOf(typeof(IEnumerable<>)) })
					.Where(p => p.CollectionType != null)
					.Select(p => new { Property = p.Property, ChildType = p.CollectionType.GetGenericArguments().First() })
					.Where(p => typeof(IContent).IsAssignableFrom(p.ChildType))
					.Select(p => p.Property)
					.ToList());

				var cacheEntry = CreateCacheEntry(content.Id, actualContentType, path, parentEntry, item.Index);

				foreach (var collectionProperty in collectionProperties)
				{
					var children = (IEnumerable<IContent>)collectionProperty.GetValue(content, null);
					RebuildPathCache(children, path, cacheEntry);
				}
			}
		}

		private ContentPathCacheEntry CreateCacheEntry(int contentId, Type contentType, ContentPath path, ContentPathCacheEntry parent, int sortOrder)
		{
			var cacheEntry = new ContentPathCacheEntry
			{
				ContentId = contentId,
				ContentType = contentType.FullName,
				Parent = parent,
				SortOrder = sortOrder,
			};

			SetPath(cacheEntry, path);

			ContentPathCacheEntries.Add(cacheEntry);

			return cacheEntry;
		}

		private void SetPath(ContentPathCacheEntry cacheEntry, ContentPath path)
		{
			SetPath(cacheEntry, path.ToString());
		}

		private static void SetPath(ContentPathCacheEntry cacheEntry, string pathString)
		{
			cacheEntry.Path = pathString;
			cacheEntry.Hash = KnuthHash.CalculateHash(pathString);
		}
	}

	internal class ContentPathCacheEntryDto
	{
		public int ContentId { get; set; }
		public string Path { get; set; }
		public string ContentType { get; set; }
		public bool HasChildren { get; set; }
	}

	internal static class ContentPathCacheEntryQueryableExtensions
	{
		public static IQueryable<ContentPathCacheEntryDto> ToContentPathCacheEntryDto(this IQueryable<ContentPathCacheEntry> query)
		{
			return query.Select(e => new ContentPathCacheEntryDto
			{
				ContentId = e.ContentId,
				ContentType = e.ContentType,
				Path = e.Path,
				HasChildren = e.Children.Any(),
			});
		}
	}
}