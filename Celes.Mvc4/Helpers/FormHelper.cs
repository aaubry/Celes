using Celes.Common;
using Celes.Mvc4.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.ComponentModel;

namespace Celes.Mvc4.Helpers
{
	public static class FormHelper
	{
		private static bool ShouldShow(ModelMetadata metadata, TemplateInfo templateInfo)
		{
			return metadata.ShowForDisplay
				&& metadata.ModelType != typeof(EntityState)
				//&& !metadata.IsComplexType
				&& !templateInfo.Visited(metadata);
		}

		public static IHtmlString RenderEditForm(this HtmlHelper<IContentInfo> html)
		{
			return (IHtmlString)_renderEditFormHelperMethod
				.MakeGenericMethod(html.ViewData.Model.ContentType)
				.Invoke(null, new object[] { html });
		}

		private static readonly MethodInfo _renderEditFormHelperMethod = ReflectionUtility
			.GetGenericMethod(() => RenderEditFormHelper<IContent>(null));

		private static IHtmlString RenderEditFormHelper<TContent>(HtmlHelper<IContentInfo> html)
			where TContent : IContent
		{
			var viewData = html.ViewContext.ViewData;
			var templateInfo = viewData.TemplateInfo;
			var modelMetadata = viewData.ModelMetadata;

			if (modelMetadata.Model == null)
			{
				return MvcHtmlString.Empty;
			}

			var contentInfo = (IContentInfo)viewData.Model;
			var helper = new HtmlHelper<IContentInfo<TContent>>(html.ViewContext, html.ViewDataContainer);

			var contentProperties = modelMetadata.Properties
				.Single(p => p.PropertyName == "Content")
				.Properties
				.Select(p => new { Metadata = p, Property = contentInfo.ContentType.GetProperty(p.PropertyName) })
				.Where(p => p.Property != null && ShouldShow(p.Metadata, templateInfo))
				.Select(p =>
				{
					var propertyModel = new PropertyModel
					{
						Property = p.Property,
						Metadata = p.Metadata,
						ContentPath = contentInfo.Path,
						Value = p.Property.GetValue(contentInfo.Content, null),
					};

					var displayAttribute = p.Property.GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().FirstOrDefault();
					if (displayAttribute != null)
					{
						propertyModel.GroupName = displayAttribute.GetGroupName();
					}

					var collectionType = p.Property.PropertyType.GetImplementationOf(typeof(ICollection<>));
					if (collectionType != null)
					{
						var collectionElementType = collectionType.GetGenericArguments().First();
						if (typeof(IContent).IsAssignableFrom(collectionElementType))
						{
							propertyModel.CollectionElementTypes = collectionElementType.Assembly
								.GetTypes()
								.Where(t => t.IsClass && !t.IsAbstract && collectionElementType.IsAssignableFrom(t))
								.Select(t => new CollectionElementTypeModel
								{
									FullName = t.FullName,
									DisplayName = GetLocalizedTypeName(t),
								})
								.ToList();
						}
					}

					if (!propertyModel.IsContentCollection)
					{
						var renderedProperty = (RenderedProperty)_renderEditFormFieldHelperMethod
							.MakeGenericMethod(contentInfo.ContentType, p.Property.PropertyType)
							.Invoke(null, new object[] { helper, p.Property });

						propertyModel.Editor = renderedProperty.Editor;
						propertyModel.ValidationMessages = renderedProperty.ValidationMessages;
					}

					return propertyModel;
				})
				.ToList();

			return html.Partial("Celes.Properties", contentProperties);
		}

		private static string GetLocalizedTypeName(Type type)
		{
			var descriptionAttribute = (DescriptionAttribute)type
				.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)
				.FirstOrDefault();

			if (descriptionAttribute != null)
			{
				return descriptionAttribute.Description;
			}

			return type.Name;
		}

		private static readonly MethodInfo _renderEditFormFieldHelperMethod = ReflectionUtility
			.GetGenericMethod(() => RenderEditFormFieldHelper<IContent, object>(null, null));

		private static RenderedProperty RenderEditFormFieldHelper<TContent, TProperty>(HtmlHelper<IContentInfo<TContent>> helper, PropertyInfo property)
			where TContent : IContent
		{
			var accessParameter = Expression.Parameter(typeof(IContentInfo<TContent>), "m");
			var accessExpression = Expression.Lambda<Func<IContentInfo<TContent>, TProperty>>(
				Expression.Property(
					Expression.Property(accessParameter, "Content"),
					property
				),
				accessParameter
			);

			return new RenderedProperty
			{
				Editor = helper.EditorFor(accessExpression),
				ValidationMessages = helper.ValidationMessageFor(accessExpression),
			};
		}

		private class RenderedProperty
		{
			public IHtmlString Editor { get; set; }
			public IHtmlString ValidationMessages { get; set; }
		}
	}
}