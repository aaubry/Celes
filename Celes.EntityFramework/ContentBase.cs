using Celes.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Celes.EntityFramework
{
	internal class RequiredAliasAttribute : RequiredAttribute
	{
		public override bool IsValid(object value)
		{
			return true;
		}
	}

	public abstract class ContentBase : IContent, IValidatableObject
	{
		[ScaffoldColumn(false)]
		public virtual int Id { get; set; }

		[Display(Order = -1)]
		[RequiredAlias]
		[MaxLength(100)]
		public virtual string Alias { get; set; }

		IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
		{
			return Validate(validationContext);
		}

		protected virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (string.IsNullOrEmpty(Alias))
			{
				var rawAlias = Convert.ToString(AliasProviderProperty.GetValue(this, null));
				if (string.IsNullOrEmpty(rawAlias))
				{
					yield return new ValidationResult("The Alias field is required.", new[] { "Alias" });
				}

				Alias = AliasGenerator.GenerateAlias(rawAlias);
			}
		}

		private PropertyInfo AliasProviderProperty
		{
			get
			{
				var aliasProviderProperties = GetType()
					.GetProperties(BindingFlags.Public | BindingFlags.Instance)
					.Where(p => p.GetCustomAttributes(typeof(GeneratesAliasAttribute), true).Any());

				try
				{
					return aliasProviderProperties.SingleOrDefault();
				}
				catch (InvalidOperationException err)
				{
					throw new InvalidOperationException(
						string.Format(
							"More than one properties have an GeneratesAliasAttribute:\n",
							string.Join("", aliasProviderProperties.Select(p => string.Format("\n  - {0}.{1}", p.DeclaringType.Name, p.Name)))
						),
						err
					);
				}
			}
		}
	}
}
