using System.Collections.Generic;
using System;

namespace Celes.Mvc4.Models
{
	public class FileManagerPathSegment
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public bool IsLast { get; set; }
	}

	public class FileManagerTreeNode
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public int FileCount { get; set; }
		public IEnumerable<FileManagerTreeNode> Children { get; set; }
	}

	public class FileManagerDirectoryEntry
	{
		public string Name { get; set; }
		public string BaseName { get; set; }
		public string Extension { get; set; }
		public string Path { get; set; }
		public string Url { get; set; }
		public long Size { get; set; }
		public DateTime Date { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
	}
}
