using Cysharp.Threading.Tasks;
using General.DTO.Entities.GameData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.GameData.Scripts
{
    internal static class AddressableHelper
    {
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

        public static async UniTask PreLoadAssets()
        {
            List<UniTask> preloadAdressableAssets = new()
            {
                Addressables.LoadAssetAsync<Sprite>("button_with_arrow_v2").ToUniTask(),
                Addressables.LoadAssetAsync<Sprite>("button_with_arrow_v2_reverse").ToUniTask()
            };

            foreach (DtoBaseHero i in G.Game.GameData.GetDtoContainer().DtoBaseHeroes)
            {
                string _name = i.Name.ToLower();
                preloadAdressableAssets.Add(Addressables.LoadAssetAsync<Sprite>($"hero-image-{_name}").ToUniTask());
                preloadAdressableAssets.Add(Addressables.LoadAssetAsync<Sprite>($"hero-image-{_name}_face").ToUniTask());
            }

            for (int i = 1; i <= 6; i++)
            {
                preloadAdressableAssets.Add(Addressables.LoadAssetAsync<Sprite>($"rarity{i}").ToUniTask());
            }

            foreach (DtoBaseEquipment i in G.Game.GameData.GetDtoContainer().DtoBaseEquipments)
            {
                string _name = i.Name.ToLower();
                string key1 = $"equipment-image-{_name}";
                string key2 = $"equipment-image-{_name}_128";
                if (await CheckIfKeyExists(key1))
                {
                    preloadAdressableAssets.Add(Addressables.LoadAssetAsync<Sprite>(key1).ToUniTask());
                }

                if (await CheckIfKeyExists(key2))
                {
                    preloadAdressableAssets.Add(Addressables.LoadAssetAsync<Sprite>(key2).ToUniTask());
                }
            }
        }
    }
}
