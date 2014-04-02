using System.Data.Entity;

namespace Celes.EntityFramework
{
	/// <summary>
	/// Defines an extension for a db context.
	/// </summary>
	public interface IDbContextExtension
	{
		/// <summary>
		/// Adds additional metadata to the specified model.
		/// </summary>
		void OnModelCreating(DbModelBuilder modelBuilder);
	}
}