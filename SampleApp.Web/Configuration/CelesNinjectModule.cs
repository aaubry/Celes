using Celes.Common;
using Celes.EntityFramework;
using Celes.Mvc4.Services;
using Ninject.Modules;
using Ninject.Web.Common;
using SampleApp.DataModel;
using System;
using System.Data.Entity;
using Celes.Mvc4.Controllers;

namespace SampleApp.Web.Configuration
{
	public class CelesNinjectModule : NinjectModule
	{
		public override void Load()
		{
			Bind<SampleDbContext, DbContext>()
				.To<SampleDbContext>()
				.InRequestScope();

			Bind<IContentPathCache>()
				.To<DbContextContentPathCache>()
				.InRequestScope();

			Bind<IContentRepository>()
				.To<DbContextContentRepository>()
				.InRequestScope();

			Bind<IUserRepository>()
				.To<DbContextUserRepository>()
				.InRequestScope();

			Bind<IAuthorizationProvider>()
				.To<DefaultAuthorizationProvider>()
				.InSingletonScope();

			Bind<Type>()
				.ToConstant(typeof(HomePage))
				.Named("rootContentType");

			Bind<FileManagerController>()
				.ToSelf()
				.WithConstructorArgument("baseVirtualPath", @"~/Images");
		}
	}
}