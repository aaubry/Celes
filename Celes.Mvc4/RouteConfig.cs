using Celes.Mvc4.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace Celes.Mvc4
{
	public static class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			var namespaces = new[] { typeof(ContentController).Namespace };

			routes.MapRoute(
				"Celes.Sitemap",
				"sitemap.xml",
				new { action = "Sitemap", controller = "Navigation" },
				namespaces
			);

			routes.MapRoute(
				"Celes.FileManager",
				"celes/assets/Scripts/tiny_mce/plugins/images/connector/php",
				new { action = "fm", controller = "FileManager" },
				namespaces
			);

			routes.MapRoute(
				"Celes.Assets",
				"celes/assets/{*path}",
				new { action = "Get", controller = "Asset" },
				namespaces
			);

			routes.MapRoute(
				"Celes.Content",
				"celes/content/{action}/{*path}",
				new { controller = "Content" },
				new { action = "Edit|Navigation|Create|MoveUp|MoveDown" },
				namespaces
			);

			routes.MapRoute(
				"Celes.Navigation",
				"celes/navigation/{action}/{*path}",
				new { controller = "Navigation" },
				namespaces
			);

			routes.MapRoute(
				"Celes.Administration",
				"celes",
				new { action = "Index", controller = "Administration" },
				namespaces
			);

			routes.MapRoute(
				"Celes.Default",
				"celes/{controller}/{action}",
				new { controller = "Cache", action = "Index" },
				new { controller = "Cache|Setup|Authentication" },
				namespaces
			);

			routes.MapRoute(
				"Celes.ViewContent",
				"{*path}",
				new { action = "Index", controller = "Content" },
				namespaces
			);

			// Never called directly
			routes.MapRoute(
				"Celes.Menu",
				"F2732391-6CBE-444C-88D9-4AC94A2FB88C",
				new { action = "Menu", controller = "Menu" },
				namespaces
			);
		}
	}
}