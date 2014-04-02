using System;

namespace Celes.EntityFramework
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class GeneratesAliasAttribute : Attribute
	{
	}
}
