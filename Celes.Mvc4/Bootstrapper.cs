using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace Celes.Mvc4
{
	public static class Bootstrapper
	{
		public static void Initialize()
		{
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			HostingEnvironment.RegisterVirtualPathProvider(new ResourceVirtualPathProvider());
		}
	}
}