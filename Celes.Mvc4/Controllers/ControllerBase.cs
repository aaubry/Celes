using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Celes.Mvc4.Controllers
{
	public abstract class ControllerBase : Controller
	{
		protected override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);

			var cookie = Request.Cookies[AuthenticationController.CookieName];
			if (cookie != null)
			{
                try
                {
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    if (!ticket.Expired || ticket.IsPersistent)
                    {
                        HttpContext.User = new GenericPrincipal(new GenericIdentity(ticket.Name, "celes"), new string[0]);
                    }
                }
                catch (HttpException)
                {
                    // Ignore invalid tokens
                }
			}
		}
	}
}
