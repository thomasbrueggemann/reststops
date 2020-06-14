using System;
namespace Reststops.Core
{
    public static class AlphaIndex
    {
        public static string Get(int index) => $"{(char)(index + 65)}".ToLower();
    }
}
