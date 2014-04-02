using System;
using System.Reflection;
using System.Web.Mvc;

namespace Celes.Mvc4
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class MultiButtonAttribute : ActionNameSelectorAttribute
	{
		public string ButtonName { get; private set; }
		public string ActionName { get; set; }
		public string ExpectedValue { get; set; }

		public MultiButtonAttribute(string buttonName)
		{
			if (buttonName == null)
			{
				throw new ArgumentNullException("buttonName");
			}

			ButtonName = buttonName;
		}

		public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
		{
			var actualValue = controllerContext.HttpContext.Request[ButtonName];
			return actualValue != null
				&& (ActionName == null || actionName == ActionName)
				&& (ExpectedValue == null || actualValue == ExpectedValue);
		}
	}
}