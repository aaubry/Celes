using Celes.Common;
using Celes.Mvc4.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Celes.Mvc4.Helpers
{
	public static class MenuHelper
	{
		public static IHtmlString Menu<T>(this HtmlHelper<T> html, int level, object htmlAttributes, int depth = 1, string viewNamePrefix = null)
			where T : IContentInfo
		{
			return html.Menu(html.ViewData.Model.Path, level, htmlAttributes, depth, viewNamePrefix);
		}

		public static IHtmlString Menu<T>(this HtmlHelper<T> html, int level, IDictionary<string, object> htmlAttributes = null, int depth = 1, string viewNamePrefix = null)
			where T : IContentInfo
		{
			return html.Menu(html.ViewData.Model.Path, level, htmlAttributes, depth, viewNamePrefix);
		}

		public static IHtmlString Menu(this HtmlHelper html, ContentPath path, int level, object htmlAttributes, int depth = 1, string viewNamePrefix = null)
		{
			return html.Menu(path, level, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), depth, viewNamePrefix);
		}

		public static IHtmlString Menu(this HtmlHelper html, ContentPath path, int level, IDictionary<string, object> htmlAttributes = null, int depth = 1, string viewNamePrefix = null)
		{
			return html.Action("Menu", "Menu", new { path, level, depth, htmlAttributes, viewNamePrefix });
		}
	}
}