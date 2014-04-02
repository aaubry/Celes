using Celes.EntityFramework;
using System.Data.Entity;

namespace SampleApp.DataModel
{
	public class SampleDbContext : ExtensibleDbContext
	{
		public SampleDbContext()
			: base(new ContentPathCacheDbContextExtension(), new UserRepositoryDbContextExtension())
		{
		}

		public virtual IDbSet<HomePage> HomePages { get; set; }
	}
}
