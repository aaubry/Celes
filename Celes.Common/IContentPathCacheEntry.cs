using System;

namespace Celes.Common
{
	public interface IContentPathCacheEntry
	{
		int ContentId { get; }
		ContentPath Path { get; }
		Type ContentType { get; }
		bool HasChildren { get; }
	}
}