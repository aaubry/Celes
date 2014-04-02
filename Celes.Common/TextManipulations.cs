using System.Globalization;
using System.Linq;
using System.Text;

namespace Celes.Common
{
	public static class TextManipulations
	{
		public static string RemoveDiacritics(string text)
		{
			return text.Normalize(NormalizationForm.FormD)
				.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
				.Aggregate(new StringBuilder(), (sb, c) => sb.Append(c), sb => sb.ToString().Normalize(NormalizationForm.FormC));
		}
	}
}