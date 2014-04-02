using System.Collections.Generic;

namespace Celes.Mvc4.Models
{
	public class MenuModel
	{
		public IDictionary<string, object> HtmlAttributes { get; set; }
		public IEnumerable<MenuItemModel> MenuItems { get; set; }
	}

	public class MenuItemModel
	{
		public string ViewName { get; set; }
		public IContentInfo Content { get; set; }
		public IEnumerable<MenuItemModel> ChildMenuItems { get; set; }
	}
}