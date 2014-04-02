using Celes.Mvc4.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace Celes.Mvc4.Controllers
{
	public abstract class FormsAuthenticationController<TLoginModel> : Controller
		where TLoginModel : new()
	{
		[HttpGet]
		public virtual ActionResult Login(string returnUrl)
		{
			return View(ViewName, new TLoginModel());
		}

		[HttpPost]
		public ActionResult Login(TLoginModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(ViewName, model);
			}

			if (!ValidateCredentials(model))
			{
				AddInvalidCredentialsError(ModelState);
				return View(ViewName, model);
			}

			var cookieInfo = GetAuthenticationCookieInfo(model);
			SetAuthCookie(cookieInfo);

			return ActionFactory.RedirectToRelative(returnUrl);
		}

		protected virtual void SetAuthCookie(AuthenticationCookieInfo cookieInfo)
		{
			FormsAuthentication.SetAuthCookie(cookieInfo.UserName, cookieInfo.RememberCredentials);
		}

		public ActionResult Logout(string returnUrl)
		{
			ClearAuthCookie();

			return ActionFactory.RedirectToRelative(returnUrl);
		}

		protected virtual void ClearAuthCookie()
		{
			FormsAuthentication.SignOut();
		}

		protected virtual string ViewName { get { return "Login"; } }

		protected abstract bool ValidateCredentials(TLoginModel loginModel);
		protected abstract AuthenticationCookieInfo GetAuthenticationCookieInfo(TLoginModel loginModel);

		protected virtual void AddInvalidCredentialsError(ModelStateDictionary modelState)
		{
			modelState.AddModelError("Password", Resources.InvalidCredentials);
		}

		protected sealed class AuthenticationCookieInfo
		{
			public string UserName { get; private set; }
			public bool RememberCredentials { get; set; }

			public AuthenticationCookieInfo(string userName, bool rememberCredentials)
			{
				UserName = userName;
				RememberCredentials = rememberCredentials;
			}
		}
	}
}