using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Celes.Mvc4.Helpers
{
	public static class JavascriptHelper
	{
		public static IHtmlString JsString(this HtmlHelper html, string value)
		{
			var serializer = new JavaScriptSerializer();
			return MvcHtmlString.Create(serializer.Serialize(value));
		}
	}
}
