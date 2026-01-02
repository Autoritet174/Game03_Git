using Cysharp.Threading.Tasks;
using General.DTO.Entities;
using General.DTO.Entities.GameData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.GameData.Scripts
{
    internal static class AddressableCache
    {
        private static Sprite NullSprite;

        public static Sprite UI_button_with_arrow_v2;
        public static Sprite UI_button_with_arrow_v2_reverse;

        public static Sprite[] Rarityes = new Sprite[7];

        public static Dictionary<string, Sprite> Heroes = new();
        public static Dictionary<string, Sprite> Equipments = new();

        public static GameObject GroupDividerPrefabAddressableGameObject;
        public static GameObject IconCollectionElementAddressableGameObject;

        /// <summary> Выполняет параллельную предварительную загрузку ассетов. </summary>
        public static async UniTask PreLoadAssets()
        {
            //DateTime start = DateTime.Now;
            DtoContainerGameData dtoContainer = G.Game.GameData.DtoContainer;

            NullSprite = await Addressables.LoadAssetAsync<Sprite>("Null").ToUniTask();

            // 3. Подготовка коллекций (аллокация заранее известного размера)
            int heroesCount = dtoContainer.DtoBaseHeroes?.Count ?? 0;
            int equipCount = dtoContainer.DtoBaseEquipments?.Count ?? 0;

            Heroes = new Dictionary<string, Sprite>(heroesCount * 2);
            Equipments = new Dictionary<string, Sprite>(equipCount * 2);

            // Список задач. Используем Capacity для избежания лишних аллокаций списка.
            // Примерное кол-во: 2 ui + heroes*2 + 7 rarities + equip*2 + 2 prefabs
            int estimatedTasks = 15 + (heroesCount * 2) + (equipCount * 2);
            var tasks = new List<UniTask>(estimatedTasks)
            {
                // UI Elements
                SafeLoadAsync("UI-buttons-button_with_arrow_v2", s => UI_button_with_arrow_v2 = s),
                SafeLoadAsync("UI-buttons-button_with_arrow_v2_reverse", s => UI_button_with_arrow_v2_reverse = s)
            };

            // Heroes
            foreach (DtoBaseHero hero in dtoContainer.DtoBaseHeroes)
            {
                string name1 = $"Heroes-{hero.Name}";
                string name2 = $"Heroes-{hero.Name}_face";

                // Используем TryAdd для избежания крэша при дубликатах в конфиге
                tasks.Add(SafeLoadAsync(name1, s => Heroes.TryAdd(name1, s)));
                tasks.Add(SafeLoadAsync(name2, s => Heroes.TryAdd(name2, s)));
            }
            

            // Rarityes
            tasks.Add(SafeLoadAsync("UI-raritySelected", s => Rarityes[0] = s));
            for (int i = 1; i <= 6; i++)
            {
                int index = i; // capture index
                tasks.Add(SafeLoadAsync($"UI-rarity{index}", s => Rarityes[index] = s));
            }

            // Equipments
            foreach (DtoBaseEquipment equipment in dtoContainer.DtoBaseEquipments)
            {
                string tagUnique = equipment.IsUnique ? "Unique-" : string.Empty;
                string key1 = $"Equipments-{tagUnique}{equipment.Name}";
                string key2 = $"Equipments-{tagUnique}{equipment.Name}_128";

                tasks.Add(SafeLoadAsync(key1, s => Equipments.TryAdd(key1, s)));
                tasks.Add(SafeLoadAsync(key2, s => Equipments.TryAdd(key2, s)));
            }
            

            // GameObjects
            tasks.Add(LoadGameObjectAsync("GroupDividerPrefab", go => GroupDividerPrefabAddressableGameObject = go));
            tasks.Add(LoadGameObjectAsync("IconCollectionElement", go => IconCollectionElementAddressableGameObject = go));

            // Ожидание всех задач
            await UniTask.WhenAll(tasks);

            //Debug.Log($"[AddressableCache] Assets loaded in: {(DateTime.Now - start).TotalSeconds:F3} sec. Total tasks: {tasks.Count}");
        }

        /// <summary> Безопасная загрузка спрайта с проверкой существования ключа. </summary>
        /// <param name="key">Addressable Key.</param>
        /// <param name="onComplete">Action для присвоения результата.</param>
        private static async UniTask SafeLoadAsync(string key, Action<Sprite> onComplete)
        {
            try
            {
                var sprite = await Addressables.LoadAssetAsync<Sprite>(key).ToUniTask();
                // Проверка на null самого ассета (если файл битый)
                onComplete(sprite ? sprite : NullSprite!);
            }
            catch (Exception)
            {
                onComplete(NullSprite!);
                //Debug.Log(ex.Message);
            }
        }

        /// <summary> Загрузка GameObject (без фоллбэка на спрайт, так как типы разные). </summary>
        private static async UniTask LoadGameObjectAsync(string key, Action<GameObject> onComplete)
        {
            try
            {
                var go = await Addressables.LoadAssetAsync<GameObject>(key).ToUniTask();
                onComplete(go);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                throw;// роняем программу так как эти ассеты гарантировано должны быть загружены
            }
        }

        /// <summary> Проверка существования ключа в каталоге Addressables. </summary>
        public static async UniTask<bool> CheckIfKeyExists(object key)
        {
            var locations = await Addressables.LoadResourceLocationsAsync(key).ToUniTask();
            return locations != null && locations.Count > 0;
        }
    }
}
