using Celes.Mvc4;
using SampleApp.DataModel;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace SampleApp.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			Database.SetInitializer(new MigrateDatabaseToLatestVersion<SampleDbContext, DataModel.Migrations.Configuration>());

			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new CsRazorViewEngine());

			Celes.Mvc4.Bootstrapper.Initialize();
		}
	}
}