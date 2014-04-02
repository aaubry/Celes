using Celes.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celes.Tests
{
	public class TestContext : IContentContext<RootContent>
	{
		public List<RootContent> Roots { get; private set; }

		public TestContext()
		{
			Roots = new List<RootContent>();
		}

		IQueryable<RootContent> IContentContext<RootContent>.Roots { get { return Roots.AsQueryable(); } }

		public void Dispose()
		{
		}

		public IQueryable<TEntity> Set<TEntity>() where TEntity : class
		{
			if (typeof(TEntity) == typeof(RootContent))
			{
				return (IQueryable<TEntity>)Roots.AsQueryable();
			}

			if (typeof(TEntity) == typeof(ChildContent))
			{
				return (IQueryable<TEntity>)Roots
					.SelectMany(r => r.Children)
					.ToList()
					.AsQueryable();
			}

			throw new ArgumentException();
		}

		public int SaveChanges()
		{
			throw new NotImplementedException();
		}
	}

	public class RootContent : IContent
	{
		public int Id { get; set; }
		public string Alias { get; set; }
		public ICollection<ChildContent> Children { get; set; }
	}

	public class ChildContent : IContent
	{
		public int Id { get; set; }
		public string Alias { get; set; }
	}
}
