using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Celes.Mvc4.Models;
using Celes.Mvc4.Services;
using System.Web.Hosting;
using System.Drawing;

namespace Celes.Mvc4.Controllers
{
	public class FileManagerController : ControllerBase
	{
		private readonly string _baseVirtualPath;

		public FileManagerController(string baseVirtualPath)
		{
			if (string.IsNullOrEmpty(baseVirtualPath))
			{
				throw new ArgumentNullException("baseVirtualPath");
			}

			_baseVirtualPath = baseVirtualPath;
		}

		[AuthorizeAdministration, MultiButton("action", ExpectedValue = "showpath")]
		public ActionResult ShowPath(string type, string path, string @default)
		{
			string basePath;
			var directory = GetPathDirectory(path, out basePath);

			var parts = directory.FullName
				.Substring(basePath.Length)
				.Trim('/', '\\')
				.Split(Path.DirectorySeparatorChar);

			var currentPath = "";
			var segments = new List<FileManagerPathSegment>();
			foreach (var part in parts)
			{
				currentPath = currentPath + "/" + part;
				segments.Add(new FileManagerPathSegment
				{
					Name = part,
					Path = currentPath,
				});
			}
			if (segments.Count > 0)
			{
				segments[segments.Count - 1].IsLast = true;
			}

			return View("Celes.ShowPath", segments);
		}

		private DirectoryInfo GetPathDirectory(string path, out string basePath)
		{
			basePath = Server.MapPath(_baseVirtualPath);
			var directory = new DirectoryInfo(Server.MapPath(_baseVirtualPath + path));
			if (!directory.FullName.StartsWith(basePath) || !directory.Exists)
			{
				throw new HttpException(404, "Not found");
			}

			return directory;
		}

		[AuthorizeAdministration, MultiButton("action", ExpectedValue = "showtree")]
		public ActionResult ShowTree(string @default)
		{
			var model = DirToNode(new DirectoryInfo(Server.MapPath(_baseVirtualPath)), "");
			return View("Celes.ShowTree", model);
		}

		private FileManagerTreeNode DirToNode(DirectoryInfo directory, string urlPath)
		{
			return new FileManagerTreeNode
			{
				Name = directory.Name,
				Path = urlPath,
				FileCount = directory.GetFiles().Length,
				Children = directory.GetDirectories()
					.Select(d => DirToNode(d, urlPath + "/" + d.Name)),
			};
		}

		[AuthorizeAdministration, MultiButton("action", ExpectedValue = "showdir")]
		public ActionResult ShowDir(string type, string path, string @default)
		{
			string basePath;
			var directory = GetPathDirectory(path, out basePath);

			var model = directory
				.GetFiles()
				.Select(f => CreateFileEntry(path, f));

			return View("Celes.ShowDir", model);
		}

		private FileManagerDirectoryEntry CreateFileEntry(string path, FileInfo file)
		{
			var entry = new FileManagerDirectoryEntry
			{
				Name = file.Name,
				BaseName = Path.GetFileNameWithoutExtension(file.Name).TrimStart('.'),
				Extension = Path.GetExtension(file.Name),
				Path = path,
				Size = file.Length,
				Date = file.CreationTime,
			};

			var extension = file.Extension.TrimStart('.');
			if (_imageExtensions.Contains(extension))
			{
				try
				{
					using (var img = Image.FromFile(file.FullName))
					{
						entry.Width = img.Width;
						entry.Height = img.Height;
					}
				}
				catch
				{
					// Do not crash if loading the image fails
				}

				entry.Url = VirtualPathUtility.ToAbsolute(_baseVirtualPath + "/" + path + "/" + file.Name);
			}
			else
			{
				var iconVirtualPath = string.Format(_iconVirtualPathTemplate, extension);
				if (!HostingEnvironment.VirtualPathProvider.FileExists(iconVirtualPath))
				{
					iconVirtualPath = string.Format(_iconVirtualPathTemplate, "none");
				}
				entry.Url = VirtualPathUtility.ToAbsolute(iconVirtualPath.Replace("~", "~/celes"));
			}

			return entry;
		}

		private const string _iconVirtualPathTemplate = "~/assets/Scripts/tiny_mce/plugins/images/img/fileicons/{0}.png";

		private static readonly HashSet<string> _imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"png",
			"jpg",
			"jpeg",
			"gif",
		};

		[AuthorizeAdministration, MultiButton("action", ExpectedValue = "SID")]
		public ActionResult GetSessionId()
		{
			return Content("TODO");
		}

		[AuthorizeAdministration, MultiButton("action", ExpectedValue = "newfolder")]
		public ActionResult CreateDir(string type, string path, string name)
		{
			string basePath;
			var directory = GetPathDirectory(path, out basePath);

			directory.CreateSubdirectory(name);

			var tree = (FileManagerTreeNode)((ViewResult)ShowTree(null)).Model;
			var addr = (IEnumerable<FileManagerPathSegment>)((ViewResult)ShowPath(type, path + "/" + name, null)).Model;

			return View("Celes.CreateDir", Tuple.Create(tree, addr));
		}

		[AuthorizeAdministration, MultiButton("action", ExpectedValue = "delfolder")]
		public ActionResult DeleteDir(string pathtype, string path)
		{
			string basePath;
			var directory = GetPathDirectory(path, out basePath);

			directory.Delete(true);

			return Json(new { ok = "" });
		}

		[AuthorizeAdministration, MultiButton("action", ExpectedValue = "uploadfile")]
		public ActionResult UploadFiles(string pathtype, string path)
		{
			string basePath;
			var directory = GetPathDirectory(path, out basePath);

			foreach (string name in Request.Files)
			{
				var file = Request.Files[name];
				file.SaveAs(Path.Combine(directory.FullName, Path.GetFileName(file.FileName)));
			}

			return Content("");
		}

		[AuthorizeAdministration, MultiButton("action", ExpectedValue = "delfile")]
		public ActionResult DeleteFiles(string pathtype, string path, FormCollection form)
		{
			string basePath;
			var directory = GetPathDirectory(path, out basePath);

			foreach (var key in form.AllKeys.Where(k => k.StartsWith("filename")))
			{
				var fileName = Path.Combine(directory.FullName, form[key]);
				System.IO.File.Delete(fileName);
			}

			return ShowDir(pathtype, path, null);
		}
	}
}