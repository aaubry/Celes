using Celes.Common;
using Celes.Mvc4.Models;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Celes.Mvc4.Controllers
{
	public class AuthenticationController : FormsAuthenticationController<LoginModel>
	{
		private readonly IUserRepository _userRepository;

		public const string CookieName = ".celes.auth";

		public AuthenticationController(IUserRepository userRepository)
		{
			if (userRepository == null)
			{
			    throw new ArgumentNullException("userRepository");
			}
			
			_userRepository = userRepository;
		}

		public override ActionResult Login(string returnUrl)
		{
			if (_userRepository.IsEmpty())
			{
				return RedirectToRoute("Celes.Default", new
				{
					controller = "Setup",
					action = "Index",
				});
			}

			return base.Login(returnUrl);
		}

		protected override string ViewName
		{
			get { return "Celes.Login"; }
		}

		protected override bool ValidateCredentials(LoginModel loginModel)
		{
			return _userRepository.ValidateCredentials(loginModel.UserName, loginModel.Password);
		}

		protected override void SetAuthCookie(AuthenticationCookieInfo cookieInfo)
		{
			var ticket = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(cookieInfo.UserName, cookieInfo.RememberCredentials, (int)FormsAuthentication.Timeout.TotalMinutes));
			Response.Cookies.Add(new HttpCookie(CookieName, ticket) {
				Expires = cookieInfo.RememberCredentials
					? new DateTime(2099, 1, 1)
					: DateTime.Now.Add(FormsAuthentication.Timeout)
			});
		}

		protected override void ClearAuthCookie()
		{
			var cookie = new HttpCookie(CookieName, "") { Expires = new DateTime(1990, 1, 1) };
			Response.Cookies.Add(cookie);
		}

		protected override FormsAuthenticationController<LoginModel>.AuthenticationCookieInfo GetAuthenticationCookieInfo(LoginModel loginModel)
		{
			return new AuthenticationCookieInfo(loginModel.UserName, loginModel.RememberCredentials);
		}
	}
}
