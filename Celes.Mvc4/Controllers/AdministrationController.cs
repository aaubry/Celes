using Celes.Common;
using Celes.Mvc4.Services;
using System.Web.Mvc;

namespace Celes.Mvc4.Controllers
{
	public class AdministrationController : ControllerBase
	{
		private readonly IContentPathCache _contentPathCache;

		public AdministrationController(IContentPathCache contentPathCache)
		{
			_contentPathCache = contentPathCache;
		}

		[HttpGet, AuthorizeAdministration]
		public ActionResult Index()
		{
			IContentPathCacheEntry model;
			try
			{
				model = _contentPathCache.GetEntryByPath(ContentPath.Root);
			}
			catch (ContentNotFoundException)
			{
				model = null;
			}
			return View("Celes.Index", model);
		}
	}
}