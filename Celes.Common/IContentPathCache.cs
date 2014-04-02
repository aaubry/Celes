using System;
using System.Collections.Generic;

namespace Celes.Common
{
	public interface IContentPathCache
	{
		/// <summary>
		/// Gets all the entries that are present in the cache.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IContentPathCacheEntry> GetAllEntries();

		/// <summary>
		/// Gets the entry that corresponds to the specified path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		IContentPathCacheEntry GetEntryByPath(ContentPath path);

		/// <summary>
		/// Gets the entries that are direct children of the specified path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		IEnumerable<IContentPathCacheEntry> GetChildEntriesOfPath(ContentPath path);

		/// <summary>
		/// Invalidates all paths from the cache. This method should be invoked
		/// if there were external changes to the content repository.
		/// </summary>
		void Invalidate();

		/// <summary>
		/// Updates the path of the specified content.
		/// </summary>
		/// <param name="oldPath">The current path of the content.</param>
		/// <param name="newPath">The new path of the content.</param>
		void UpdatePath(ContentPath oldPath, ContentPath newPath);

		/// <summary>
		/// Removes the specified path from the cache.
		/// </summary>
		/// <param name="path"></param>
		void RemovePath(ContentPath path);

		/// <summary>
		/// Adds the specified path to the cache.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="contentId"></param>
		/// <param name="contentType"></param>
		void AddPath(ContentPath path, int contentId, Type contentType);

		/// <summary>
		/// Moves the specified content up or down in the content list.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="moveUp"></param>
		/// <returns>Returns true if the content was reordered.</returns>
		bool ReorderPath(ContentPath path, bool moveUp);
	}
}