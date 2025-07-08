using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.GameData.Scripts
{
    public static class LocalizationManager
    {
        private static Dictionary<string, string> localization = new();
        public static string Language { get; private set; }

        public static string GetValue(string key)
        {
            return localization[key];
        }

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


        public static void Init(string language)
        {
            Language = language;
            TextAsset jsonFile = Resources.Load<TextAsset>($"Localization/{language}/data");
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
