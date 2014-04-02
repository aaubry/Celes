using System;

namespace Celes.Common
{
	public interface IContentRepository
	{
		Type RootContentType { get; }
		TContent GetContentById<TContent>(int id) where TContent : class, IContent;
		void DeleteContent<TContent>(TContent content) where TContent : class, IContent;
		void AddContent<TContent>(TContent content) where TContent : class, IContent;
		void SaveChanges();
	}
}