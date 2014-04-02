using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Celes.EntityFramework
{
	/// <summary>
	/// A <see cref="DbContext"/> that allows extensions to it.
	/// </summary>
	public abstract class ExtensibleDbContext : DbContext
	{
		private readonly IEnumerable<IDbContextExtension> _extensions;

		public ExtensibleDbContext(params IDbContextExtension[] extensions)
			: this((IEnumerable<IDbContextExtension>)extensions)
		{
		}

		public ExtensibleDbContext(IEnumerable<IDbContextExtension> extensions)
		{
			if (extensions == null)
			{
				throw new ArgumentNullException("extensions");
			}

			_extensions = extensions;
		}

		public ExtensibleDbContext(string nameOrConnectionString, params IDbContextExtension[] extensions)
			: this(nameOrConnectionString, (IEnumerable<IDbContextExtension>)extensions)
		{
		}

		public ExtensibleDbContext(string nameOrConnectionString, IEnumerable<IDbContextExtension> extensions)
			: base(nameOrConnectionString)
		{
			if (extensions == null)
			{
			    throw new ArgumentNullException("extensions");
			}
			
			_extensions = extensions;
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			foreach (var extension in _extensions)
			{
				extension.OnModelCreating(modelBuilder);
			}
		}
	}
}