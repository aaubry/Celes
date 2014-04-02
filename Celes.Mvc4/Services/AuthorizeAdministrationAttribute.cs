using System.Web.Mvc;
using System.Web.Routing;

namespace Celes.Mvc4.Services
{
	internal class AuthorizeAdministrationAttribute : AuthorizeAttribute
	{
		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			filterContext.Result = new RedirectToRouteResult("Celes.Default", new RouteValueDictionary(new
			{
				action = "Login",
				controller = "Authentication",
				returnUrl = filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery,
			}));
		}
	}
}
