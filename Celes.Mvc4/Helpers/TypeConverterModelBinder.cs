using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace Celes.Mvc4.Helpers
{
	/// <summary>
	/// An implementation of <see cref="IModelBinder" /> that uses the <see cref="TypeConverter" /> of a type to perform conversions.
	/// </summary>
	public class TypeConverterModelBinder : IModelBinder
	{
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var converter = TypeDescriptor.GetConverter(bindingContext.ModelType);
			if (converter == null)
			{
				throw new ArgumentException(string.Format("Type '{0}' does not have an associated TypeConverter", bindingContext.ModelType.FullName));
			}

			var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			
			var valueType = value.RawValue != null
				? value.RawValue.GetType()
				: typeof(string);
			
			if (valueType == bindingContext.ModelType)
			{
				return value.RawValue;
			}

			if (!converter.CanConvertFrom(valueType))
			{
				throw new ArgumentException(string.Format("Type '{0}' cannot be converted from '{1}'", bindingContext.ModelType.FullName, value.RawValue.GetType().FullName));
			}

			return converter.ConvertFrom(null, value.Culture, value.RawValue);
		}
	}
}
