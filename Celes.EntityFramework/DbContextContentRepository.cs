using Celes.Common;
using System;
using System.Data.Entity;
using System.Linq;

namespace Celes.EntityFramework
{
	/// <summary>
	/// Implements <see cref="IContentRepository"/> based on a <see cref="DbContext"/>.
	/// </summary>
	public sealed class DbContextContentRepository : IContentRepository
	{
		private readonly DbContext _dbContext;

		public DbContextContentRepository(DbContext dbContext, Type rootContentType)
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
			RootContentType = rootContentType;
		}

		public Type RootContentType { get; private set; }

		public TContent GetContentById<TContent>(int id) where TContent : class, IContent
		{
			return _dbContext.Set<TContent>().FirstOrDefault(c => c.Id == id);
		}

		public void DeleteContent<TContent>(TContent content) where TContent : class, IContent
		{
			_dbContext.Set<TContent>().Remove(content);
		}

		public void AddContent<TContent>(TContent content) where TContent : class, IContent
		{
			_dbContext.Set<TContent>().Add(content);
		}

		public void SaveChanges()
		{
			_dbContext.SaveChanges();
		}
	}
}