using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    public class LocalizationManager: MonoBehaviour
    {
        public static Dictionary<string, string> Localization = new();
        private void Start()
        {
            Init();
        }
        private void Init()
        {
            string languageCode = "ru";
            TextAsset jsonFile = Resources.Load<TextAsset>($"Localization/{languageCode}/data.json");
            Localization = JsonUtility.FromJson<Dictionary<string, string>>(jsonFile.text);
        }
    }
}
