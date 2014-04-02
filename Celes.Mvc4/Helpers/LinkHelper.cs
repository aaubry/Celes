using Celes.Common;
using Celes.Mvc4.Models;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Celes.Mvc4.Helpers
{
	public static class LinkHelper
	{
		public static IHtmlString ContentLink<TModel>(this HtmlHelper<TModel> html, string linkText, ContentPath path, object htmlAttributes = null)
			where TModel : IContentInfo
		{
			return html.ActionLink(linkText, "Index", "Content", new { path }, htmlAttributes);
		}

		public static IHtmlString ContentLink<TModel>(this HtmlHelper<TModel> html, string linkText, IContent content, object htmlAttributes = null)
			where TModel : IContentInfo
		{
			return html.ContentLink(linkText, content.Alias, htmlAttributes);
		}

		public static IHtmlString ContentLink<TModel>(this HtmlHelper<TModel> html, string linkText, string contentAlias, object htmlAttributes = null)
			where TModel : IContentInfo
		{
			return html.ContentLink(linkText, html.ViewData.Model.Path.Append(contentAlias), htmlAttributes);
		}
	}
}
