using System;
using System.ComponentModel;
using System.Reflection;

namespace Celes.Common
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class LocalizedDescriptionAttribute : DescriptionAttribute
	{
		public Type ResourceType { get; private set; }

		public LocalizedDescriptionAttribute(string resourceName, Type resourceType)
			: base(resourceName)
		{
			ResourceType = resourceType;
		}

		public override string Description
		{
			get
			{
				var property = ResourceType.GetProperty(DescriptionValue, BindingFlags.Static | BindingFlags.Public);
				if (property != null)
				{
					return (string)property.GetValue(null, null);
				}
				return base.Description;
			}
		}
	}
}
