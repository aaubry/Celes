using Celes.Common;
using Celes.Mvc4.Models;
using System.Reflection;
using System.Web.Mvc;

namespace Celes.Mvc4.Controllers
{
	public abstract class ContentControllerBase : ControllerBase
	{
		protected readonly IContentRepository _contentRepository;
		protected readonly IContentPathCache _contentPathCache;

		public ContentControllerBase(IContentPathCache contentPathCache, IContentRepository contentRepository)
		{
			_contentPathCache = contentPathCache;
			_contentRepository = contentRepository;
		}

		private static readonly MethodInfo _getContentInfoMethod = ReflectionUtility
			.GetGenericMethod((ContentController c) => c.GetContentInfo<IContent>(0, null));

		private IContentInfo<TContent> GetContentInfo<TContent>(int contentId, ContentPath path)
			where TContent : class, IContent
		{
			var content = _contentRepository.GetContentById<TContent>(contentId);
			return new ContentInfo<TContent>(content, path);
		}

		protected IContentInfo GetContent(ContentPath path)
		{
			var cacheEntry = _contentPathCache.GetEntryByPath(path);
			return GetContent(cacheEntry);
		}

		protected IContentInfo GetContent(IContentPathCacheEntry cacheEntry)
		{
			return (IContentInfo)_getContentInfoMethod
				.MakeGenericMethod(cacheEntry.ContentType)
				.Invoke(this, new object[] { cacheEntry.ContentId, cacheEntry.Path });
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			if (filterContext.Exception is ContentNotFoundException)
			{
				filterContext.ExceptionHandled = true;
				filterContext.Result = HttpNotFound(filterContext.Exception.Message);
			}
			else
			{
				base.OnException(filterContext);
			}
		}
	}
}
