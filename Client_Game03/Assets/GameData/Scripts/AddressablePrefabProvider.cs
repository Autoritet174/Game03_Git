using Cysharp.Threading.Tasks;
using General.DTO.Entities;
using General.DTO.Entities.Collection;
using General.DTO.Entities.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.GameData.Scripts
{
    internal static class AddressablePrefabProvider
    {
        private static Sprite NullSprite;

        public static Sprite UI_button_with_arrow_v4;
        public static Sprite UI_button_with_arrow_v4_reverse;

        private static readonly Sprite[] Rarityes = new Sprite[7];
        public static Sprite RaritySelected { get; private set; }


        public static Dictionary<string, Sprite> Heroes = new();
        public static Dictionary<string, Sprite> Equipments = new();

        public static GameObject GroupDividerPrefabAddressableGameObject { get; private set; }
        public static GameObject IconCollectionElementAddressableGameObject;

        public static GameObject BattlefieldUnit;
        public static GameObject HealthChange;
        public static GameObject ProgressBar;

        /// <summary> Выполняет параллельную предварительную загрузку ассетов. </summary>
        public static async UniTask PreLoadAssets()
        {
            //DateTime start = DateTime.Now;
            DtoContainerGameData dtoContainer = Game03Client.GameData.Container;

            NullSprite = await Addressables.LoadAssetAsync<Sprite>("Null").ToUniTask();

            // 3. Подготовка коллекций (аллокация заранее известного размера)
            int heroesCount = dtoContainer.baseHeroes.Count();
            int equipCount = dtoContainer.baseEquipments.Count();

            Heroes = new Dictionary<string, Sprite>(heroesCount * 2);
            Equipments.Clear();

            // Список задач. Используем Capacity для избежания лишних аллокаций списка.
            // Примерное кол-во: 2 ui + heroes*2 + 7 rarities + equip*2 + 2 prefabs
            int estimatedTasks = 15 + (heroesCount * 2) + equipCount;
            var tasks = new List<UniTask>(estimatedTasks)
            {
                // UI Elements
                SafeLoadAsync("button_with_arrow_v4", s => UI_button_with_arrow_v4 = s),
                SafeLoadAsync("button_with_arrow_v4_reverse", s => UI_button_with_arrow_v4_reverse = s)
            };

            // Heroes
            foreach (BaseHero hero in dtoContainer.baseHeroes)
            {
                // Используем TryAdd для избежания крэша при дубликатах в конфиге
                tasks.Add(SafeLoadAsync($"Heroes-{hero.name}", s => Heroes.TryAdd(hero.name, s)));
                tasks.Add(SafeLoadAsync($"Heroes-{hero.name}_face", s => Heroes.TryAdd($"{hero.name}_face", s)));
            }


            // Rarityes
            tasks.Add(SafeLoadAsync("UI-raritySelected", s => RaritySelected = s));
            for (int i = 1; i <= 6; i++)
            {
                int index = i; // capture index
                tasks.Add(SafeLoadAsync($"UI-rarity{index}", s => Rarityes[index] = s));
            }
            //Rarityes_v2[0] = NullSprite;
            //for (int i = 1; i <= 6; i++)
            //{
            //    int index = i; // capture index
            //    tasks.Add(SafeLoadAsync($"UI-rarity{index}_v2", s => Rarityes_v2[index] = s));
            //}


            // Equipments
            foreach (BaseEquipment equipment in dtoContainer.baseEquipments)
            {
                tasks.Add(SafeLoadAsync($"Equipments-{equipment.name}", s => Equipments.TryAdd(equipment.name, s)));
            }


            // GameObjects
            tasks.Add(LoadGameObjectAsync("GroupDividerPrefab", go => GroupDividerPrefabAddressableGameObject = go));
            tasks.Add(LoadGameObjectAsync("IconCollectionElement", go => IconCollectionElementAddressableGameObject = go));
            tasks.Add(LoadGameObjectAsync("BattlefieldUnit", go => BattlefieldUnit = go));
            tasks.Add(LoadGameObjectAsync("HealthChange", go => HealthChange = go));
            tasks.Add(LoadGameObjectAsync("ProgressBar", go => ProgressBar = go));

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

        ///// <summary> Проверка существования ключа в каталоге Addressables. </summary>
        //public static async UniTask<bool> CheckIfKeyExists(object key)
        //{
        //    var locations = await Addressables.LoadResourceLocationsAsync(key).ToUniTask();
        //    return locations != null && locations.Count > 0;
        //}
        public static Sprite GetRarity(int rarity)
        {
            return Rarityes[rarity];
        }

        public static Sprite GetHeroSprite(Hero hero)
        {
            BaseHero baseHero = hero.baseHero!;
            return Heroes[baseHero.name];
        }
        public static Sprite GetHeroFaceSprite(Hero hero)
        {
            BaseHero baseHero = hero.baseHero!;
            return Heroes[$"{baseHero.name}_face"];
        }

        public static Sprite GetEquipmentSprite(Equipment equipment)
        {
            BaseEquipment baseEquipment = equipment.baseEquipment!;
            return Equipments[baseEquipment.name];
        }
    }
}
