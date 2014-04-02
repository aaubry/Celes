using Celes.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SampleApp.DataModel
{
	public class HomePage : ContentBase
	{
		[Display(Order = 1, Name = "Text", Description = "The text that is displayed inside the page.")]
		[UIHint("RichText")]
		public virtual string Text { get; set; }

		[Display(Order = 2, Name = "Pages", Description = "The child pages.", GroupName = "Pages")]
		public virtual ICollection<TextPage> Pages { get; set; }
	}
}