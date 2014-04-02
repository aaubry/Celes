using Celes.Common;
using System.Text.RegularExpressions;

namespace Celes.EntityFramework
{
	public static class AliasGenerator
	{
		public static string GenerateAlias(string text)
		{
			text = TextManipulations.RemoveDiacritics(text);
			text = text.ToLowerInvariant();
			text = Regex.Replace(text, @"[^\w]+", "-", RegexOptions.Compiled);
			return text.Trim('-');
		}
	}
}