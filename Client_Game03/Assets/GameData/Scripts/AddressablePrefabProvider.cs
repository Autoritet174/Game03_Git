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
    /// <summary>Предварительно загружает и хранит спрайты и префабы игрового интерфейса.</summary>
    internal static class AddressablePrefabProvider
    {
        private static Sprite nullSprite;

        public static Sprite ui_button_with_arrow_v4;
        public static Sprite ui_button_with_arrow_v4_reverse;

        private static readonly Sprite[] rarityes = new Sprite[7];
        public static Sprite raritySelected { get; private set; }

        public static Dictionary<string, Sprite> heroes = new();
        public static Dictionary<string, Sprite> equipments = new();

        public static GameObject groupDividerPrefabAddressableGameObject { get; private set; }

        public static GameObject iconCollectionElementAddressableGameObject;

        public static GameObject battlefieldUnit;
        public static GameObject healthChange;
        public static GameObject progressBar;

        #region Загрузка ресурсов

        /// <summary>Выполняет параллельную предварительную загрузку ассетов.</summary>
        public static async UniTask PreLoadAssetsAsync()
        {
            DtoContainerGameData dtoContainer = Game03Client.GameData.Container;

            nullSprite = await Addressables.LoadAssetAsync<Sprite>("Null").ToUniTask();

            // 3. Подготовка коллекций (аллокация заранее известного размера)
            int heroesCount = dtoContainer.baseHeroes.Count();
            int equipCount = dtoContainer.baseEquipments.Count();

            heroes = new(heroesCount * 2);
            equipments.Clear();

            // Список задач. Используем Capacity для избежания лишних аллокаций списка.
            // Примерное кол-во: 2 ui + heroes*2 + 7 rarities + equip*2 + 2 prefabs
            int estimatedTasks = 15 + (heroesCount * 2) + equipCount;
            List<UniTask> tasks = new(estimatedTasks)
            {
                // Элементы интерфейса
                SafeLoadAsync("button_with_arrow_v4", s => ui_button_with_arrow_v4 = s),
                SafeLoadAsync("button_with_arrow_v4_reverse", s => ui_button_with_arrow_v4_reverse = s)
            };

            // Герои
            foreach (BaseHero hero in dtoContainer.baseHeroes)
            {
                // Используем TryAdd для избежания крэша при дубликатах в конфиге
                tasks.Add(SafeLoadAsync($"Heroes-{hero.name}", s => heroes.TryAdd(hero.name, s)));
                tasks.Add(SafeLoadAsync($"Heroes-{hero.name}_face", s => heroes.TryAdd($"{hero.name}_face", s)));
            }

            // Редкость
            tasks.Add(SafeLoadAsync("UI-raritySelected", s => raritySelected = s));
            for (int i = 1; i <= 6; i++)
            {
                int index = i; // Сохраняем индекс для отложенного обработчика.
                tasks.Add(SafeLoadAsync($"UI-rarity{index}", s => rarityes[index] = s));
            }

            // Экипировка
            foreach (BaseEquipment equipment in dtoContainer.baseEquipments)
            {
                tasks.Add(SafeLoadAsync($"Equipments-{equipment.name}", s => equipments.TryAdd(equipment.name, s)));
            }

            // Префабы интерфейса
            tasks.Add(LoadGameObjectAsync("GroupDividerPrefab", go => groupDividerPrefabAddressableGameObject = go));
            tasks.Add(LoadGameObjectAsync("IconCollectionElement", go => iconCollectionElementAddressableGameObject = go));
            tasks.Add(LoadGameObjectAsync("BattlefieldUnit", go => battlefieldUnit = go));
            tasks.Add(LoadGameObjectAsync("HealthChange", go => healthChange = go));
            tasks.Add(LoadGameObjectAsync("ProgressBar", go => progressBar = go));

            // Ожидание всех задач
            await UniTask.WhenAll(tasks);
        }

        /// <summary>Безопасная загрузка спрайта с проверкой существования ключа.</summary>
        /// <param name="key">Ключ ресурса Addressables.</param>
        /// <param name="onComplete">Action для присвоения результата.</param>
        private static async UniTask SafeLoadAsync(string key, Action<Sprite> onComplete)
        {
            try
            {
                var sprite = await Addressables.LoadAssetAsync<Sprite>(key).ToUniTask();
                // Проверка на null самого ассета (если файл битый)
                onComplete(sprite ? sprite : nullSprite!);
            }
            catch (Exception)
            {
                onComplete(nullSprite!);
            }
        }

        /// <summary>Загрузка GameObject (без фоллбэка на спрайт, так как типы разные).</summary>
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

        #endregion Загрузка ресурсов

        #region Получение ресурсов

        ///// <summary>Проверка существования ключа в каталоге Addressables.</summary>
        //public static async UniTask<bool> CheckIfKeyExists(object key)
        //{
        //    var locations = await Addressables.LoadResourceLocationsAsync(key).ToUniTask();
        //    return locations != null && locations.Count > 0;
        //}
        public static Sprite GetRarity(int rarity)
        {
            return rarityes[rarity];
        }

        public static Sprite GetHeroSprite(Hero hero)
        {
            BaseHero baseHero = hero.baseHero!;
            return heroes[baseHero.name];
        }

        public static Sprite GetHeroFaceSprite(Hero hero)
        {
            BaseHero baseHero = hero.baseHero!;
            return heroes[$"{baseHero.name}_face"];
        }

        public static Sprite GetEquipmentSprite(Equipment equipment)
        {
            BaseEquipment baseEquipment = equipment.baseEquipment!;
            return equipments[baseEquipment.name];
        }

        #endregion Получение ресурсов
    }
}
