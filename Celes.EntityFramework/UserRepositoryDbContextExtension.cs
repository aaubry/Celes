using System;
using System.Data.Entity;

namespace Celes.EntityFramework
{
	/// <summary>
	/// Creates the entities required to use <see cref="DbContextUserRepository"/> with a <see cref="DbContext"/>.
	/// </summary>
	public sealed class UserRepositoryDbContextExtension : IDbContextExtension
	{
		public void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>();
		}
	}
}