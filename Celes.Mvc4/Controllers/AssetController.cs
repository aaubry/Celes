using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Celes.Mvc4.Controllers
{
	public class AssetController : ControllerBase
	{
		private static readonly IDictionary<string, string> _contentTypes = new Dictionary<string, string>
		{
			{ ".js", "text/javascript" },
			{ ".css", "text/css" },
			{ ".gif", "image/gif" },
			{ ".jpg", "image/jpeg" },
			{ ".jpeg", "image/jpeg" },
			{ ".png", "image/png" },
			{ ".htm", "text/html" },
			{ ".html", "text/html" },
			{ ".xml", "text/xml" },
		};

		[HttpGet]
		public ActionResult Get(string path)
		{
			var resourceName = typeof(Bootstrapper).Namespace + ".Assets." + path.Replace('/', '.');

			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

			string contentType;
			if(!_contentTypes.TryGetValue(Path.GetExtension(path), out contentType))
			{
				contentType = "text/plain";
			}

			Response.Cache.SetExpires(DateTime.Now.AddYears(1));
			Response.Cache.SetCacheability(HttpCacheability.Public);

			return new FileStreamResult(stream, contentType);
		}
	}
}