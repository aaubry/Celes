using Celes.Common;
using System.Collections.Generic;

namespace Celes.Mvc4.Models
{
	public class NavigationModel
	{
		public IEnumerable<IContentPathCacheEntry> Siblings { get; set; }
		public ContentPath CurrentPath { get; set; }
	}
}