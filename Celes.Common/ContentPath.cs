using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Celes.Common
{
	[TypeConverter(typeof(ContentPathConverter))]
	public sealed class ContentPath : IEquatable<ContentPath>, IEnumerable<string>
	{
		public sealed class ContentPathConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
			{
				return ContentPath.Parse((string)value);
			}
		}

		private const char ContentPathSeparator = '/';

		private readonly string[] _path;

		private ContentPath(string[] path)
		{
			_path = path;
		}

		public ContentPath Append(string segment)
		{
			var newPath = new string[_path.Length + 1];
			Array.Copy(_path, newPath, _path.Length);
			newPath[_path.Length] = segment;
			return new ContentPath(newPath);
		}

		public override bool Equals(object obj)
		{
			return obj is ContentPath && Equals((ContentPath)obj);
		}

		public bool Equals(ContentPath other)
		{
			if (other == null)
			{
				return false;
			}

			if (_path.Length != other._path.Length)
			{
				return false;
			}

			for (int i = 0; i < _path.Length; ++i)
			{
				if (_path[i] != other._path[i])
				{
					return false;
				}
			}

			return true;
		}

		public override int GetHashCode()
		{
			if (_path.Length == 0)
			{
				return 0;
			}

			return _path
				.Select(p => p.GetHashCode())
				.Aggregate(CombineHashCodes);
		}

		private static int CombineHashCodes(int h1, int h2)
		{
			return ((h1 << 5) + h1) ^ h2;
		}

		public override string ToString()
		{
			return string.Join(new string(ContentPathSeparator, 1), _path);
		}

		public static readonly ContentPath Root = new ContentPath(new string[0]);

		public IEnumerator<string> GetEnumerator()
		{
			return _path.AsEnumerable().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public static ContentPath Parse(string path)
		{
			return string.IsNullOrEmpty(path)
				? Root
				: new ContentPath(path.TrimStart(ContentPathSeparator).Split(ContentPathSeparator));
		}

		public ContentPath GetParent()
		{
			switch (_path.Length)
			{
				case 0:
					return null;

				case 1:
					return Root;

				default:
					var newPath = new string[_path.Length - 1];
					Array.Copy(_path, newPath, newPath.Length);
					return new ContentPath(newPath);
			}
		}

		public IEnumerable<ContentPath> GetSegments()
		{
			yield return Root;

			for (int i = 1; i < _path.Length; ++i)
			{
				var segment = new string[i];
				Array.Copy(_path, segment, i);
				yield return new ContentPath(segment);
			}
		}

		public ContentPath ReplaceLastSegment(string newSegment)
		{
			if (_path.Length == 0)
			{
				return Root;
			}

			var newPath = (string[])_path.Clone();
			newPath[newPath.Length - 1] = newSegment;
			return new ContentPath(newPath);
		}

		/// <summary>
		/// Returns the number of segments in the path.
		/// </summary>
		public int Count
		{
			get
			{
				return _path.Length;
			}
		}

		public bool IsRoot
		{
			get
			{
				return Equals(Root);
			}
		}
	}
}