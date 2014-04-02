using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Celes.Mvc4.Models
{
	public class CreateUserModel : IValidatableObject
	{
		[Required]
		[StringLength(20)]
		[Display(ResourceType = typeof(Resources), Name = "UserName", Order = 1)]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[StringLength(100, MinimumLength = 7, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PasswordTooShort")]
		[Display(ResourceType = typeof(Resources), Name = "Password", Order = 2)]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(ResourceType = typeof(Resources), Name = "PasswordConfirmation", Order = 3)]
		public string PasswordConfirmation { get; set; }

		IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
		{
			if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(PasswordConfirmation) && Password != PasswordConfirmation)
			{
				yield return new ValidationResult(Resources.PasswordsDoNotMatch, new[] { "PasswordConfirmation" });
			}
		}
	}
}
