using Celes.Common;
using Celes.Mvc4.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Celes.Mvc4.Controllers
{
	public class NavigationController : ControllerBase
	{
		private readonly IContentPathCache _contentPathCache;

		public NavigationController(IContentPathCache contentPathCache)
		{
			_contentPathCache = contentPathCache;
		}

		[HttpGet]
		public ActionResult Children([ModelBinder(typeof(TypeConverterModelBinder))] ContentPath path)
		{
			return GetNavigationJson(_contentPathCache.GetChildEntriesOfPath(path));
		}

		private ActionResult GetNavigationJson(IEnumerable<IContentPathCacheEntry> entries)
		{
			var nodes = entries.Select(e => new
			{
				data = new
				{
					title = e.Path == ContentPath.Root
						? Resources.RootContentName
						: e.Path.Last(),
				},
				attr = new
				{
					path = e.Path.ToString(),
				},
				state = e.HasChildren ? "closed" : null,
			});

			return Json(nodes, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult Sitemap()
		{
			var baseUrl = new Uri(Request.Url, "/");
			var paths = _contentPathCache.GetAllEntries();

			const string siteMapNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";

			var document = new XDocument(
				new XElement(XName.Get("urlset", siteMapNamespace),

					paths.Select(p => new XElement(XName.Get("url", siteMapNamespace),
						new XElement(XName.Get("loc", siteMapNamespace),
							string.Format("{0}{1}", baseUrl.ToString().TrimEnd('/'), p.Path)
						)
					))
				)
			);

			return Content(document.ToString(), "text/xml");
		}
	}
}