
namespace Celes.EntityFramework
{
	internal static class KnuthHash
	{
		public static long CalculateHash(string text)
		{
			unchecked
			{
				ulong hashedValue = 3074457345618258791ul;
				for (int i = 0; i < text.Length; ++i)
				{
					hashedValue += text[i];
					hashedValue *= 3074457345618258799ul;
				}
				return (long)hashedValue;
			}
		}
	}
}