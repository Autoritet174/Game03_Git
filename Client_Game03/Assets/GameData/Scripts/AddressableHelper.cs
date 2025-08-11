using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}
