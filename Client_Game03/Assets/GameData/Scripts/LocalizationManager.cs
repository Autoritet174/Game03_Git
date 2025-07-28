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
        /// Получить строку которая является ключом к тексту ошибки
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public static string GetKeyError(JObject jObject)
        {
            General.ServerErrors.Error err = General.ServerErrors.Error.Unknown;
            try
            {
                long code = long.Parse(jObject["code"]?.ToString() ?? ((long)General.ServerErrors.Error.Unknown).ToString());
                err = General.ServerErrors.GetResponse(code);
            }
            catch { }

            string key = err switch
            {
                General.ServerErrors.Error.Auth_EmailOrPassword_Empty => "Errors.EmailOrPassword_Empty",
                General.ServerErrors.Error.Auth_EmailAndPassword_NotFound => "Errors.EmailAndPassword_NotFound",
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
