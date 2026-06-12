using System;
using System.IO;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    /// <summary>
    /// Dev-only prefill credentials from gitignored Main.dev.ini (see Main.dev.ini.example).
    /// </summary>
    public static class DevAuthConfig
    {
        private const string CONFIG_RELATIVE_PATH = "GameData/Config/Main.dev.ini";
        private const string SECTION_AUTH = "Auth";
        private const string KEY_EMAIL = "Email";
        private const string KEY_PASSWORD = "Password";

        public static bool TryGetPrefillCredentials(out string email, out string password)
        {
            email = null;
            password = null;

#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            return false;
#endif
            string path = Path.Combine(Application.dataPath, CONFIG_RELATIVE_PATH);
            if (!File.Exists(path))
            {
                return false;
            }

            return TryParseAuthSection(path, out email, out password);
        }

        private static bool TryParseAuthSection(string path, out string email, out string password)
        {
            email = null;
            password = null;
            bool inAuthSection = false;

            foreach (string rawLine in File.ReadAllLines(path))
            {
                string line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";"))
                {
                    continue;
                }

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    inAuthSection = string.Equals(line, $"[{SECTION_AUTH}]", StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (!inAuthSection)
                {
                    continue;
                }

                int eqIndex = line.IndexOf('=');
                if (eqIndex <= 0)
                {
                    continue;
                }

                string key = line.Substring(0, eqIndex).Trim();
                string value = line.Substring(eqIndex + 1).Trim();

                if (string.Equals(key, KEY_EMAIL, StringComparison.OrdinalIgnoreCase))
                {
                    email = value;
                }
                else if (string.Equals(key, KEY_PASSWORD, StringComparison.OrdinalIgnoreCase))
                {
                    password = value;
                }
            }

            return !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password);
        }
    }
}
