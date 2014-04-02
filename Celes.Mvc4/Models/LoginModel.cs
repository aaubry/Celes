using System.ComponentModel.DataAnnotations;

namespace Celes.Mvc4.Models
{
	public class LoginModel
	{
		[Required]
		[Display(ResourceType = typeof(Resources), Name = "UserName", Order = 1)]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(ResourceType = typeof(Resources), Name = "Password", Order = 2)]
		public string Password { get; set; }

		[Display(ResourceType = typeof(Resources), Name = "RememberMe", Order = 3)]
		public bool RememberCredentials { get; set; }
	}
}