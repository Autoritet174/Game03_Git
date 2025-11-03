using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    public static class LocalizationManager
    {
        public enum Languages
        {
            [Description("en")]
            English = 1,

            [Description("ru")]
            Russian = 2
        }

        /// <summary>
        /// Минимальная процедура получения строки из Description конкретного значения Languages.
        /// </summary>
        /// <param name="language">Значение перечисления Languages.</param>
        /// <returns>Строка из атрибута Description.</returns>
        private static string GetLanguageCode(Languages language)
        {
            FieldInfo field = typeof(Languages).GetField(language.ToString());
            DescriptionAttribute attr = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attr?.Description ?? throw new InvalidOperationException("DescriptionAttribute not found.");
        }

        /// <summary>
        /// Язык интерфейса.
        /// </summary>
        public static Languages Language
        {
            get => language;
            set
            {
                language = value;
                Init();
            }
        }

        private static Dictionary<string, string> localization = new();
        private static Languages language;

        /// <summary>
        /// Получаем локализованный текст, если не удается найти возвращаем ключ.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            return localization.TryGetValue(key, out string value) ? value : key;
        }


        /// <summary>
        /// Получить строку которая является ключом к тексту ошибки.
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public static string GetKeyError(JObject jObject, Dictionary<string, string> argDict)
        {
            long code = long.Parse(jObject["code"]?.ToString() ?? ((long)General.ServerErrors.Error.Unknown).ToString());


            // Аккаунт забанен
            if (code == (long)General.ServerErrors.Error.AccountBannedUntil)
            {
                string dateTimeExpiresAtString = (jObject["dateTimeExpiresAt"]?.ToString() ?? string.Empty).Trim();
                if (dateTimeExpiresAtString != string.Empty)
                {
                    argDict.Add("{datetimeExpiration}", dateTimeExpiresAtString);

                    string[] dtA = dateTimeExpiresAtString.Split(new string[] { " ", ".", ":" }, StringSplitOptions.None);
                    try
                    {
                        DateTime dtUnbanUtc = new(int.Parse(dtA[0]), int.Parse(dtA[1]), int.Parse(dtA[2]), int.Parse(dtA[3]), int.Parse(dtA[4]), int.Parse(dtA[5]));
                        long secondsRemaining = (long)(dtUnbanUtc - DateTime.UtcNow).TotalSeconds;
                        argDict.Add("{timeRemaining}", secondsRemaining > 0 ? StringExtensions.SecondsToTimeStr(secondsRemaining) : "0");
                    }
                    catch { }
                }
            }


            // Много попыток входа
            if (code == (long)General.ServerErrors.Error.TooManyRequests)
            {
                string secondsRemainingString = (jObject["secondsRemaining"]?.ToString() ?? string.Empty).Trim();
                if (secondsRemainingString != string.Empty && long.TryParse(secondsRemainingString, out long secondsRemaining) && secondsRemaining > 0)
                {
                    argDict.Add("{timeRemaining}", StringExtensions.SecondsToTimeStr(secondsRemaining));
                }
            }

            General.ServerErrors.Error err = General.ServerErrors.GetResponse(code);

            string key = err switch
            {
                General.ServerErrors.Error.AuthInvalidCredentials => "Errors.AuthInvalidCredentials",
                General.ServerErrors.Error.TooManyRequests => "Errors.TooManyRequests",
                General.ServerErrors.Error.AccountBannedUntil => "Errors.AccountBannedUntil",
                General.ServerErrors.Error.AccountBannedPermanently => "Errors.AccountBannedPermanently",
                _ => "Errors.Server_UnknownError",
            };

            return key;
        }


        private static void Init()
        {
            TextAsset jsonFile = Resources.Load<TextAsset>($"Localization/{GetLanguageCode(language)}/data");
            localization.Clear();
            localization = ParseDeepJson(jsonFile.text);
        }


        private static Dictionary<string, string> ParseDeepJson(string jsonText)
        {
            Dictionary<string, string> result = new();
            JObject obj = JObject.Parse(jsonText);

            void ProcessToken(JToken token, string currentPath)
            {
                switch (token.Type)
                {
                    case JTokenType.Object:
                        foreach (JProperty prop in token.Children<JProperty>())
                        {
                            ProcessToken(prop.Value, $"{currentPath}{prop.Name}.");
                        }
                        break;
                    case JTokenType.String:
                        result[currentPath.TrimEnd('.')] = token.ToString();
                        break;
                }
            }

            ProcessToken(obj, "");
            return result;
        }

    }
}
