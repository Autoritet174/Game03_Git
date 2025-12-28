using Cysharp.Threading.Tasks;
using General.DTO.Entities.GameData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.GameData.Scripts
{
    internal static class AddressableHelper
    {
        public static Sprite Null;
        public static Sprite UI_button_with_arrow_v2;
        public static Sprite UI_button_with_arrow_v2_reverse;

        public static Sprite[] Rarityes = new Sprite[7];

        public static Dictionary<string, Sprite> Heroes = new();
        public static Dictionary<string, Sprite> Equipments = new();

        public static async UniTask PreLoadAssets()
        {
            Null = await Addressables.LoadAssetAsync<Sprite>("null").ToUniTask();

            UI_button_with_arrow_v2 = await LoadAsync("button_with_arrow_v2");
            UI_button_with_arrow_v2_reverse = await LoadAsync("button_with_arrow_v2_reverse");

            foreach (DtoBaseHero i in G.Game.GameData.GetDtoContainer().DtoBaseHeroes)
            {
                string _name = i.Name;
                Heroes.Add(_name, await LoadAsync($"hero-image-{_name}"));
                Heroes.Add(_name + "_face", await LoadAsync($"hero-image-{_name}_face"));
            }

            Rarityes[0] = await LoadAsync($"raritySelected");
            for (int i = 1; i <= 6; i++)
            {
                Rarityes[i] = await LoadAsync($"rarity{i}");
            }

            foreach (DtoBaseEquipment i in G.Game.GameData.GetDtoContainer().DtoBaseEquipments)
            {
                string _name = i.Name;
                string key1 = $"equipment-image-{_name}";
                string key2 = $"equipment-image-{_name}_128";
                Equipments.Add(_name, await LoadAsync(key1));
                Equipments.Add(_name + "_128", await LoadAsync(key2));
            }
        }


        /// <summary>
        /// Проверка существует ли ключ в addressable.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async UniTask<bool> CheckIfKeyExists(string key)
        {
            // Загружаем информацию об ассете по ключу
            var locations = await Addressables.LoadResourceLocationsAsync(key).ToUniTask();

            // Если locations не пустой, ключ существует
            return locations.Count > 0;
        }

        public static async UniTask<Sprite> LoadAsync(string name)
        {
            return await CheckIfKeyExists(name) ? await Addressables.LoadAssetAsync<Sprite>(name).ToUniTask() : Null;
        }
    }
}
