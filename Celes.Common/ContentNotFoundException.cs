using System;

namespace Celes.Common
{
	[Serializable]
	public class ContentNotFoundException : Exception
	{
		public ContentPath Path { get; private set; }

		public ContentNotFoundException(ContentPath path)
			: base(string.Format("Content with path '{0}' not found.", path))
		{
			Path = path;
		}

		public ContentNotFoundException() { }
		public ContentNotFoundException(string message) : base(message) { }
		public ContentNotFoundException(string message, Exception inner) : base(message, inner) { }
		protected ContentNotFoundException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
