using System.Collections.Generic;
using System.Reflection;
using System.Web.Hosting;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Web;

namespace Celes.Mvc4
{
	internal class ResourceVirtualPathProvider : VirtualPathProvider
	{
		private readonly Assembly _resourceAssembly;
		private readonly string _resourcePrefix;
		private readonly Dictionary<string, string> _resourceNames;

		public ResourceVirtualPathProvider()
		{
			_resourceAssembly = typeof(ResourceVirtualPathProvider).Assembly;
			_resourcePrefix = typeof(Bootstrapper).Namespace;

			_resourceNames = _resourceAssembly
				.GetManifestResourceNames()
				.ToDictionary(n => n, StringComparer.InvariantCultureIgnoreCase);
		}

		private bool ParseVirtualPath(string virtualPath, out string resourceName)
		{
			resourceName = _resourcePrefix + virtualPath.Replace('/', '.').TrimStart('~');
			return _resourceNames.TryGetValue(resourceName, out resourceName);
		}

		public override bool FileExists(string virtualPath)
		{
			string resourceName;
			return
				base.FileExists(virtualPath) ||
				ParseVirtualPath(virtualPath, out resourceName) ||
				// Handle default content view
				Regex.IsMatch(virtualPath, "^/Views/Content/[^/]+$");
		}

		public override VirtualFile GetFile(string virtualPath)
		{
			if (base.FileExists(virtualPath))
			{
				return base.GetFile(virtualPath);
			}
			else
			{
				string resourceName;
				if (!ParseVirtualPath(virtualPath, out resourceName))
				{
					// Handle default content view
					resourceName = _resourcePrefix + ".Views.Content.Celes.ContentDefault.cshtml";
				}
				return new ResourceVirtualFile(_resourceAssembly, resourceName, virtualPath);
			}
		}

		public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, System.DateTime utcStart)
		{
			return base.FileExists(virtualPath)
				? base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart)
				: null;
		}

		private class ResourceVirtualFile : VirtualFile
		{
			private readonly Assembly _resourceAssembly;
			private readonly string _resourceName;

			public ResourceVirtualFile(Assembly resourceAssembly, string resourceName, string virtualPath)
				: base(virtualPath)
			{
				_resourceAssembly = resourceAssembly;
				_resourceName = resourceName;
			}

			public override System.IO.Stream Open()
			{
				var resourceStream = _resourceAssembly.GetManifestResourceStream(_resourceName);
#if DEBUG
				if (_resourceName.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
				{
					var basePath = Environment.GetEnvironmentVariable("CELES_SOURCE");
					if (!string.IsNullOrEmpty(basePath))
					{
						var fileName = Path.Combine(
							Path.Combine(basePath, "Celes.Mvc4"),
							VirtualPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
						var header = string.Format("@{{ #line 2 \"{0}\" }}\n", fileName);

						var text = header + new StreamReader(resourceStream).ReadToEnd();
						resourceStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
					}
				}
#endif
				return resourceStream;
			}
		}

		private sealed class ConcatStream : Stream
		{
			private readonly IEnumerator<Stream> _streams;
			private bool _isEmpty;

			public ConcatStream(IEnumerable<Stream> streams)
			{
				_streams = streams.GetEnumerator();
				_isEmpty = !_streams.MoveNext();
			}

			public override bool CanRead
			{
				get { return true; }
			}

			public override bool CanSeek
			{
				get { return false; }
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				if (_isEmpty)
				{
					return 0;
				}

				int bytesRead = _streams.Current.Read(buffer, offset, count);
				if (bytesRead != 0)
				{
					return bytesRead;
				}

				_streams.Current.Dispose();
				_isEmpty = !_streams.MoveNext();

				return Read(buffer, offset, count);
			}

			public override bool CanWrite
			{
				get { return false; }
			}

			public override void Flush()
			{
				throw new NotSupportedException();
			}

			public override long Length
			{
				get { throw new NotSupportedException(); }
			}

			public override long Position
			{
				get
				{
					throw new NotSupportedException();
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}
		}
	}
}