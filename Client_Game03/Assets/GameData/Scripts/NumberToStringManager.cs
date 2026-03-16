using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.GameData.Scripts
{
    public static class NumberToStringManager
    {
        public static string NumberToShortString(float n)
        {
            if (n < 100)
            {
                return n.ToString().Substring(0, Math.Min(4, n.ToString().Length));
            }
            else if (n < 1000)
            {
                return n.ToString().Substring(0, Math.Min(3, n.ToString().Length));
            }
            else if (n < 100000)
            {
                return (n / 1000).ToString().Substring(0, Math.Min(4, (n / 1000).ToString().Length)) + "K";
            }
            else if (n < 1000000)
            {
                return (n / 1000).ToString().Substring(0, Math.Min(3, (n / 1000).ToString().Length)) + "K";
            }
            else if (n < 100000000)
            {
                return (n / 1000000).ToString().Substring(0, Math.Min(4, (n / 1000000).ToString().Length)) + "M";
            }
            else if (n < 1000000000)
            {
                return (n / 1000000).ToString().Substring(0, Math.Min(3, (n / 1000000).ToString().Length)) + "M";
            }
            else if (n < 100000000000)
            {
                return (n / 1000000000).ToString().Substring(0, Math.Min(4, (n / 1000000000).ToString().Length)) + "B";
            }
            else if (n < 1000000000000)
            {
                return (n / 1000000000).ToString().Substring(0, Math.Min(3, (n / 1000000000).ToString().Length)) + "B";
            }
            else
            {
                return n.ToString("0.00e0").Replace("e+", "e");
            }
        }
    }
}
