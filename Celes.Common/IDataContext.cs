using System;

namespace Celes.Common
{
	public interface IDataContext : IDisposable
	{
		int SaveChanges();
	}
}
