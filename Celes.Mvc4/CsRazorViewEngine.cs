using System.Linq;
using System.Web.Mvc;

namespace Celes.Mvc4
{
	/// <summary>
	/// A ViewEngine that suports only cshtml files.
	/// </summary>
	public class CsRazorViewEngine : RazorViewEngine
	{
		public CsRazorViewEngine()
			: this(null)
		{
		}

		public CsRazorViewEngine(IViewPageActivator viewPageActivator)
			: base(viewPageActivator)
		{
			AreaViewLocationFormats = AreaViewLocationFormats.Where(f => f.EndsWith("cshtml")).ToArray();
			AreaMasterLocationFormats = AreaMasterLocationFormats.Where(f => f.EndsWith("cshtml")).ToArray();
			AreaPartialViewLocationFormats = AreaPartialViewLocationFormats.Where(f => f.EndsWith("cshtml")).ToArray();
			ViewLocationFormats = ViewLocationFormats.Where(f => f.EndsWith("cshtml")).ToArray();
			MasterLocationFormats = MasterLocationFormats.Where(f => f.EndsWith("cshtml")).ToArray();
			PartialViewLocationFormats = PartialViewLocationFormats.Where(f => f.EndsWith("cshtml")).ToArray();
			FileExtensions = FileExtensions.Where(f => f.EndsWith("cshtml")).ToArray();
		}
	}
}