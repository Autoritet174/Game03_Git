namespace Assets.GameData.Scripts
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Если возможно, преобразуем первый символ в верхний регистр, остальные без изменений.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToUpper1Char(this string s)
        {
            return string.IsNullOrWhiteSpace(s) || s.Length == 0 ? s : $"{s[..1].ToUpper()}{s[1..]}";
        }

        public static string SecondsToTimeStr(long sec) {
            long min = sec / 60L;
            sec -= min * 60L;

            long hours = min / 60L;
            min -= hours * 60L;

            long days = hours / 24L;
            hours -= days * 24L;

            string result = $"{min:00}m {sec:00}s";
            if (days > 0)
            {
                result = $"{days}d {hours:00}h {result}";
            }
            else if (hours > 0)
            {
                result = $"{hours:00}h {result}";
            }

            return result;
        }
    }
}
