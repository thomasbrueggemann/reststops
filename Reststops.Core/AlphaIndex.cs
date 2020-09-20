using System;
namespace Reststops.Core
{
    public static class AlphaIndex
    {
        public static string Get(int index)
        {
            if (index < 0 || index > 25)
                return string.Empty;

            return $"{(char)(index + 65)}".ToLower();
        }
    }
}
