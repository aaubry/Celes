using Celes.Common;
using Celes.Mvc4.Helpers;
using Celes.Mvc4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Celes.Mvc4.Controllers
{
	public class MenuController : ContentControllerBase
	{
		public MenuController(IContentPathCache contentPathCache, IContentRepository contentRepository)
			: base(contentPathCache, contentRepository)
		{
		}

		[ChildActionOnly]
		public ActionResult Menu([ModelBinder(typeof(TypeConverterModelBinder))] ContentPath path, int level, int depth, IDictionary<string, object> htmlAttributes, string viewNamePrefix)
		{
			if (level < 1)
			{
				throw new ArgumentOutOfRangeException("level", "level must be positive");
			}

			var endLevel = level + depth;
			while (path.Count >= endLevel)
			{
				path = path.GetParent();
			}

			var currentLevelItems = _contentPathCache.GetChildEntriesOfPath(path)
				.Select(e => new MenuItemModel
				{
					ViewName = (viewNamePrefix ?? "") + e.ContentType.Name,
					Content = GetContent(e),
				})
				.ToList();

			while (path.Count >= level)
			{
				var childPath = path;
				path = path.GetParent();

				currentLevelItems = _contentPathCache.GetChildEntriesOfPath(path)
					.Select(e => new MenuItemModel
					{
						ViewName = (viewNamePrefix ?? "") + e.ContentType.Name,
						Content = GetContent(e),
						ChildMenuItems = e.Path.Equals(childPath) ? currentLevelItems : null,
					})
					.ToList();
			}

			return PartialView("Celes.Menu", new MenuModel
			{
				HtmlAttributes = htmlAttributes,
				MenuItems = currentLevelItems //GetMenuItemForPath(path, viewNamePrefix, depth),
			});
		}

		private IList<MenuItemModel> GetMenuItemForPath(ContentPath path, string viewNamePrefix, int depth)
		{
			if (depth <= 0)
			{
				return null;
			}

			return _contentPathCache.GetChildEntriesOfPath(path)
				.Select(e => new MenuItemModel
				{
					ViewName = (viewNamePrefix ?? "") + e.ContentType.Name,
					Content = GetContent(e),
					ChildMenuItems = GetMenuItemForPath(e.Path, viewNamePrefix, depth - 1),
				})
				.ToList();
		}
	}
}