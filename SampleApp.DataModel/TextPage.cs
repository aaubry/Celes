using Celes.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SampleApp.DataModel
{
	[Description("Text page")]
	public class TextPage : ContentBase
	{
		[GeneratesAlias]
		public virtual string Title { get; set; }

		[UIHint("RichText")]
		public virtual string Text { get;set;}

		public virtual ICollection<TextPage> Pages { get; set; }
	}
}
