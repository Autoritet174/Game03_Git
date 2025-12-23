using General.DTO.Entities.GameData;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public static async Task<bool> CheckIfKeyExists(string key)
        {
            // Загружаем информацию об ассете по ключу
            IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> locations = await Addressables.LoadResourceLocationsAsync(key).Task;

            // Если locations не пустой, ключ существует
            return locations.Count > 0;
        }

        public static async Task PreLoadAssets()
        {
            List<Task> preloadAdressableAssets = new()
            {
                Addressables.LoadAssetAsync<Sprite>("button_with_arrow_v2").Task,
                Addressables.LoadAssetAsync<Sprite>("button_with_arrow_v2_reverse").Task
            };

            foreach (DtoBaseHero i in G.Game.GameData.GetDtoContainer().DtoBaseHeroes)
            {
                string _name = i.Name.ToLower();
                preloadAdressableAssets.Add(Addressables.LoadAssetAsync<Sprite>($"hero-image-{_name}").Task);
                preloadAdressableAssets.Add(Addressables.LoadAssetAsync<Sprite>($"hero-image-{_name}_face").Task);
            }

            for (int i = 1; i <= 6; i++)
            {
                preloadAdressableAssets.Add(Addressables.LoadAssetAsync<Sprite>($"rarity{i}").Task);
            }

            foreach (DtoBaseEquipment i in G.Game.GameData.GetDtoContainer().DtoBaseEquipments)
            {
                string _name = i.Name.ToLower();
                string key1 = $"equipment-image-{_name}";
                string key2 = $"equipment-image-{_name}_icon";
                if (await CheckIfKeyExists(key1))
                {
                    preloadAdressableAssets.Add(Addressables.LoadAssetAsync<Sprite>(key1).Task);
                }

                if (await CheckIfKeyExists(key2))
                {
                    preloadAdressableAssets.Add(Addressables.LoadAssetAsync<Sprite>(key2).Task);
                }
            }
        }
    }
}
