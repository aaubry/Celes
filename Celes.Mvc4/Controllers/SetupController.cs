using Celes.Common;
using Celes.Mvc4.Models;
using System.Web;
using System.Web.Mvc;

namespace Celes.Mvc4.Controllers
{
	public class SetupController : ControllerBase
	{
		private readonly IUserRepository _userRepository;

		public SetupController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		protected override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (!Request.IsLocal)
			{
				throw new HttpException(403, "Only local requests are authorized");
			}

			base.OnAuthorization(filterContext);
		}

		[HttpGet]
		public ActionResult Index()
		{
			return View("Celes.Index", new CreateUserModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public ActionResult CreateUser(CreateUserModel model)
		{
			if (ModelState.IsValid)
			{
				_userRepository.CreateUser(model.UserName, model.Password);
				return RedirectToAction("Index", "Administration");
			}

			return View("Celes.Index", new CreateUserModel());
		}
	}
}