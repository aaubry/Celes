using Celes.Common;
using System.Data.Entity;

namespace Celes.EntityFramework
{
	/// <summary>
	/// Creates the entities required to use <see cref="DbContextContentPathCache"/> with a <see cref="DbContext"/>.
	/// </summary>
	public sealed class ContentPathCacheDbContextExtension : IDbContextExtension
	{
		public void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ContentPathCacheEntry>()
				.HasMany(e => e.Children)
				.WithOptional(e => e.Parent)
				.WillCascadeOnDelete(false);
		}
	}
}