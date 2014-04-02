using Celes.Common;
using System;

namespace Celes.Mvc4.Models
{
	public interface IContentInfo
	{
		IContent Content { get; }
		Type ContentType { get; }
		ContentPath Path { get; }
	}

	public interface IContentInfo<out TContent> : IContentInfo
		where TContent : IContent
	{
		new TContent Content { get; }
	}
}
