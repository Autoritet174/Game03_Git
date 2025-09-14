using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Assets.GameData.Scripts
{
    internal static class StringExtensions
    {
        public static string ToUpper1Char(this string s)
        {
            if (string.IsNullOrWhiteSpace(s) || s.Length == 0)
            {
                return s;
            }
            return $"{s[..1].ToUpper()}{s[1..]}";
        }
    }
}
