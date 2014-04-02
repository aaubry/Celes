using Celes.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Celes.Mvc4.Models
{
	public class PropertyModel
	{
		public ContentPath ContentPath { get; set; }
		public ModelMetadata Metadata { get; set; }
		public PropertyInfo Property { get; set; }
		public IHtmlString Editor { get; set; }
		public IHtmlString ValidationMessages { get; set; }
		public string GroupName { get; set; }
		public bool IsContentCollection { get { return CollectionElementTypes != null && CollectionElementTypes.Any(); } }
		public IEnumerable<CollectionElementTypeModel> CollectionElementTypes { get; set; }
		public object Value { get; set; }
	}

	public class CollectionElementTypeModel
	{
		public string FullName { get; set; }
		public string DisplayName { get; set; }
	}
}
