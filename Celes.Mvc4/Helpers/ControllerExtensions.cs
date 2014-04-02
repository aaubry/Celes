using System;
using System.Web.Mvc;

namespace Celes.Mvc4.Helpers
{
	public static class ActionFactory
	{
		public static ActionResult RedirectToRelative(string url)
		{
			if (!Uri.IsWellFormedUriString(url, UriKind.Relative))
			{
				url = "/";
			}

			return new RedirectResult(url);
		}
	}
}
