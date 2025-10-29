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
    }
}
