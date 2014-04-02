using Celes.Common;
using System;
using System.Reflection;

namespace Celes.Mvc4.Models
{
	internal class ContentInfo : IContentInfo
	{
		public IContent Content { get; private set; }
		public Type ContentType { get; private set; }
		public ContentPath Path { get; private set; }

		public ContentInfo(IContent content, Type contentType, ContentPath path)
		{
			Content = content;
			ContentType = contentType;
			Path = path;
		}

		private static readonly MethodInfo _createHelperMethod = ReflectionUtility
			.GetGenericMethod(() => CreateHelper<IContent>(null, null));

		private static IContentInfo<TContent> CreateHelper<TContent>(TContent content, ContentPath path)
			where TContent : IContent
		{
			return new ContentInfo<TContent>(content, path);
		}

		public static ContentInfo Create(IContent content, Type contentType, ContentPath path)
		{
			return (ContentInfo)_createHelperMethod
				.MakeGenericMethod(contentType)
				.Invoke(null, new object[] { content, path });
		}
	}

	internal class ContentInfo<TContent> : ContentInfo, IContentInfo<TContent>
		where TContent : IContent
	{
		public new TContent Content { get { return (TContent)base.Content; } }

		public ContentInfo(TContent content, ContentPath path)
			: base(content, typeof(TContent), path)
		{
		}
	}
}
