using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.GameData.Scenes.BattleField
{
    public static class BattleFieldData
    {

        private static string locationName;
        public static void Load(string locationName)
        {
            BattleFieldData.locationName = locationName;
        }
    }
}
