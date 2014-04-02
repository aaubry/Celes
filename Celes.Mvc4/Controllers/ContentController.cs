using Celes.Common;
using Celes.Mvc4.Helpers;
using Celes.Mvc4.Models;
using Celes.Mvc4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Celes.Mvc4.Controllers
{
	public class ContentController : ContentControllerBase
	{
		private readonly IAuthorizationProvider _authorizationProvider;

		public ContentController(IContentPathCache contentPathCache, IContentRepository contentRepository, IAuthorizationProvider authorizationProvider)
			: base(contentPathCache, contentRepository)
		{
			_authorizationProvider = authorizationProvider;
		}

		[HttpGet]
		public ActionResult Index([ModelBinder(typeof(TypeConverterModelBinder))] ContentPath path)
		{
			try
			{
				var contentInfo = GetContent(path);
				if (!_authorizationProvider.IsAuthorizedToView(ControllerContext.HttpContext.User, contentInfo))
				{
					return new HttpUnauthorizedResult();
				}

				var viewName = contentInfo.ContentType.Name;
				return View(viewName, contentInfo);
			}
			catch (ContentNotFoundException)
			{
				if (path == ContentPath.Root)
				{
					return RedirectToAction("Index", "Administration");
				}
				throw;
			}
		}

		private IContentInfo GetContentOrNewRoot(ContentPath path)
		{
			try
			{
				return GetContent(path);
			}
			catch (ContentNotFoundException)
			{
				if (path == ContentPath.Root)
				{
					var content = (IContent)Activator.CreateInstance(_contentRepository.RootContentType);

					ReflectionUtility
						.GetGenericMethod(() => _contentRepository.AddContent<IContent>(null))
						.MakeGenericMethod(_contentRepository.RootContentType)
						.Invoke(_contentRepository, new object[] { content });

					return ContentInfo.Create(content, _contentRepository.RootContentType, null);
				}
				else
				{
					throw;
				}
			}
		}

		[HttpGet, AuthorizeAdministration]
		public ActionResult Edit([ModelBinder(typeof(TypeConverterModelBinder))] ContentPath path)
		{
			var contentInfo = GetContentOrNewRoot(path);
			return View("Celes.Edit", contentInfo);
		}

		private static readonly MethodInfo _tryUpdateModelMethod = ReflectionUtility
			.GetGenericMethod((ContentController c) => c.TryUpdateModel<object>(null, ""));

		[HttpPost, AuthorizeAdministration, ValidateInput(false), ValidateAntiForgeryToken, MultiButton("save", ActionName = "Edit")]
		public ActionResult Edit([ModelBinder(typeof(TypeConverterModelBinder))] ContentPath path, FormCollection formValues)
		{
			var contentInfo = GetContentOrNewRoot(path);
			return UpdateContent(
				contentInfo,
				s => path.ReplaceLastSegment(s),
				newPath => RedirectToAction("Edit", new { path = newPath })
			);
		}

		[HttpPost, AuthorizeAdministration, ValidateInput(false), ValidateAntiForgeryToken, MultiButton("delete", ActionName = "Edit")]
		public ActionResult Delete([ModelBinder(typeof(TypeConverterModelBinder))] ContentPath path, FormCollection formValues)
		{
			var contentInfo = GetContent(path);
			if (contentInfo.Path.IsRoot)
			{
				throw new HttpException(403, "The root content cannot be deleted.");
			}

			_deleteContentMethod
				.MakeGenericMethod(contentInfo.ContentType)
				.Invoke(_contentRepository, new object[] { contentInfo.Content });

			_contentPathCache.RemovePath(contentInfo.Path);

			SetWarningMessage(Resources.DeletedSuccessfully);
			return View("Celes.Deleted", contentInfo);
		}

		private static readonly MethodInfo _deleteContentMethod = ReflectionUtility
			.GetGenericMethod((IContentRepository r) => r.DeleteContent<IContent>(null));

		[HttpGet, AuthorizeAdministration]
		public ActionResult Create([ModelBinder(typeof(TypeConverterModelBinder))] ContentPath path, string container, string type = null)
		{
			var contentInfo = CreateNewContent(path, container, type);
			return View("Celes.Edit", contentInfo);
		}

		[HttpPost, AuthorizeAdministration, ValidateInput(false), ValidateAntiForgeryToken]
		public ActionResult Create([ModelBinder(typeof(TypeConverterModelBinder))] ContentPath path, string container, string type, FormCollection formValues)
		{
			var contentInfo = CreateNewContent(path, container, type);
			return UpdateContent(
				contentInfo,
				s => path.Append(s),
				newPath => View("Celes.Created", newPath)
			);
		}

		[HttpPost, AuthorizeAdministration, ValidateInput(false)]
		public ActionResult MoveUp([ModelBinder(typeof(TypeConverterModelBinder))] ContentPath path)
		{
			var success = _contentPathCache.ReorderPath(path, true);
			return Content(success.ToString().ToLowerInvariant());
		}

		[HttpPost, AuthorizeAdministration, ValidateInput(false)]
		public ActionResult MoveDown([ModelBinder(typeof(TypeConverterModelBinder))] ContentPath path)
		{
			var success = _contentPathCache.ReorderPath(path, false);
			return Content(success.ToString().ToLowerInvariant());
		}

		private ActionResult UpdateContent(IContentInfo contentInfo, Func<string, ContentPath> makeNewPath, Func<ContentPath, ActionResult> onSuccess)
		{
			var modelUpdated = (bool)_tryUpdateModelMethod
				.MakeGenericMethod(contentInfo.ContentType)
				.Invoke(this, new object[] { contentInfo.Content, "Content" });

			if (modelUpdated)
			{
				_contentRepository.SaveChanges();

				var newPath = makeNewPath(contentInfo.Content.Alias);
				if (contentInfo.Path != null)
				{
					if (!newPath.Equals(contentInfo.Path))
					{
						_contentPathCache.UpdatePath(contentInfo.Path, newPath);
					}
				}
				else
				{
					_contentPathCache.AddPath(newPath, contentInfo.Content.Id, contentInfo.ContentType);
				}

				SetSuccessMessage(Resources.SavedSuccessfully);

				return onSuccess(newPath);
			}
			else
			{
				SetErrorMessage(Resources.ThereWereValidationErrors);

				return View("Celes.Edit", contentInfo);
			}
		}

		private ContentInfo CreateNewContent(ContentPath path, string container, string type)
		{
			var parentContentInfo = GetContent(path);

			var property = parentContentInfo.ContentType.GetProperty(container);
			if (property == null)
			{
				throw new HttpException(404, "Invalid container");
			}

			var collectionType = property.PropertyType.GetImplementationOf(typeof(ICollection<>));
			if (collectionType == null)
			{
				throw new HttpException(400, "Not a collection");
			}

			var collectionItemType = collectionType.GetGenericArguments().First();
			var contentType = collectionItemType.Assembly
				.GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract && collectionItemType.IsAssignableFrom(t))
				.Single(t => t.FullName == type);

			var content = (IContent)Activator.CreateInstance(contentType);

			// Add the new content to its parent collection.
			var collectionInstance = property.GetValue(parentContentInfo.Content, null);
			collectionType
				.GetMethod("Add")
				.Invoke(collectionInstance, new object[] { content });

			return ContentInfo.Create(content, contentType, null);
		}

		private void SetSuccessMessage(string message)
		{
			SetMessage(message, "success");
		}

		private void SetWarningMessage(string message)
		{
			SetMessage(message, "warning");
		}

		private void SetErrorMessage(string message)
		{
			SetMessage(message, "error");
		}

		private void SetMessage(string message, string type)
		{
			TempData["Celes.MessageType"] = type;
			TempData["Celes.Message"] = message;
		}
	}
}