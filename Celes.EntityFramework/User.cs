using Celes.Common;
using System.ComponentModel.DataAnnotations;

namespace Celes.EntityFramework
{
	public class User : IUser
	{
		public virtual int Id { get; set; }

		[Required, MaxLength(20)]
		public virtual string UserName { get; set; }

		[Required]
		public virtual string PasswordHash { get; set; }
	}
}
