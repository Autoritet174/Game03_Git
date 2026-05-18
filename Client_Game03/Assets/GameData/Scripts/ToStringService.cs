namespace Assets.GameData.Scripts
{
    public static class ToStringService
    {
        public static int mode = 1;
        public static int modePercent = 1;

        /// <summary>
        /// В строку формата Game03
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string ToStr(this float n)
        {
            return mode switch
            {
                1 => n.ToString("0"),
                2 => ToStr3(n),
                _ => n.ToString(),
            };
        }

        public static string ToStrPercent(this float n)
        {
            return (modePercent switch
            {
                1 => n.ToString("0.0"),
                2 => ToStr3(n),
                _ => n.ToString(),
            }) + "%";
        }


        private static readonly char[] suffix = { 'K', 'M', 'B', 'T', 'Q' };
        private static readonly int suffix_Length = suffix.Length;

        private static string ToStr3(float n)
        {
            // Обработка специальных случаев
            if (float.IsNaN(n) || float.IsInfinity(n))
            {
                return n.ToString();
            }

            if (n == 0f)
            {
                return "0";
            }

            bool negative = n < 0f;
            float value = negative ? -n : n;

            if (value < 0.1f)
            {
                string result = value.ToString("0.00e0").Replace("e+", "e").Replace("E+", "e");
                return negative ? $"-{result}" : result;
            }

            if (value < 1f)
            {
                string result = value.ToString("0.###");
                return negative ? $"-{result}" : result;
            }

            string formatted = "";
            float originalValue = value; // Сохраняем для возможного научного формата
            int i;
            for (i = -1; i < suffix_Length; i++)
            {
                if (value < 10f)
                {
                    formatted = value.ToString("0.##");
                    break;
                }
                if (value < 100f)
                {
                    formatted = value.ToString("0.#");
                    break;
                }
                if (value < 1000f)
                {
                    formatted = value.ToString("0");
                    break;
                }

                value /= 1000f;
            }

            // Если число слишком большое для наших суффиксов
            if (formatted == "")
            {
                // Используем оригинальное значение для научной нотации
                formatted = originalValue.ToString("0.00e0").Replace("e+", "e").Replace("E+", "e");
            }
            else if (i > -1 && i < suffix_Length)
            {
                formatted += suffix[i];
            }

            return negative ? $"-{formatted}" : formatted;
        }

    }
}
