using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celes.EntityFramework
{
	[Table(TableName)]
	public class ContentPathCacheEntry
	{
		internal const string TableName = "ContentPathCacheEntries";

		public virtual long Hash { get; set; }

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public virtual int Id { get; set; }

		public virtual int SortOrder { get; set; }

		public virtual ContentPathCacheEntry Parent { get; set; }

		public virtual ICollection<ContentPathCacheEntry> Children { get; set; }

		public virtual int ContentId { get; set; }

		[MaxLength(200)]
		[Required]
		public virtual string ContentType { get; set; }

		[MaxLength]
		[Required(AllowEmptyStrings = true)]
		public virtual string Path { get; set; }
	}
}