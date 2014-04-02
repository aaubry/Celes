using Celes.Common;
using Celes.Mvc4.Services;
using System.Web.Mvc;

namespace Celes.Mvc4.Controllers
{
	public class CacheController : ControllerBase
	{
		private readonly IContentPathCache _contentPathCache;

		public CacheController(IContentPathCache contentPathCache)
		{
			_contentPathCache = contentPathCache;
		}

		[HttpGet, AuthorizeAdministration]
		public ActionResult Invalidate()
		{
			_contentPathCache.Invalidate();
			return Content("Cache rebuilt");
		}
	}
}
