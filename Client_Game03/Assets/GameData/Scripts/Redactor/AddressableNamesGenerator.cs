using General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Assets.GameData.Scripts.Redactor
{
    public static class AddressableNamesGenerator
    {

        private const string START_DIR = "Assets/GameData/AddressableAssets/Images";
        private const string GROUP_NAME_GENERATED_SPRITES = "Auto Generated Sprites";
        private static readonly string[] Dirs = { "Heroes", "Equipment", "UI", "SmithingMaterials" };
        private static readonly string[] Formats = { ".jpg", ".png" };

        [MenuItem("_Game03/Генерировать имена addressable")]
        public static void Generate()
        {
            string[] files = Directory.GetFiles(START_DIR, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].ToDirectSlash();
            }

            List<string> dirsList = new();
            foreach (string dir in Dirs)
            {
                dirsList.Add(Path.Combine(START_DIR, dir).ToDirectSlash().ToLowerInvariant());
            }

            List<string> filesList = new(files.Where(file =>
            {
                string fileL = file.ToLowerInvariant();
                return Formats.Any(format => fileL.EndsWith(format))
                                && dirsList.Any(dir => fileL.StartsWith(dir));
            }));
            foreach (string file in filesList)
            {
                string assetPathInput = file[(START_DIR.Length + 1)..];
                string assetPath = Path.Combine(START_DIR, assetPathInput).ToDirectSlash();
                string assetAddressableName = assetPathInput.Replace('/', '-');
                if (Formats.Any(format => assetAddressableName.EndsWith(format)))
                {
                    int indexLastComma = assetAddressableName.LastIndexOf('.');
                    assetAddressableName = assetAddressableName[..indexLastComma];
                }

                RegisterInAddressables(assetPath, assetAddressableName, GROUP_NAME_GENERATED_SPRITES);
                //RegisterInAddressables(smallAssetPath, assetAddressableName + suffix, GROUP_NAME_GENERATED_SPRITES);

                AssetDatabase.SaveAssets();
            }
            //CreateDualSprites("Equipment/_Unique/thunderfury.jpg", EQUIPMENT_PREFIX_SMALL);

            // Выводим уведомление в центре экрана редактора
            _ = EditorUtility.DisplayDialog(nameof(SpriteGenerator), "Генерация спрайтов успешно завершена!", "OK");
        }


        /// <summary>
        /// Добавляет ассет в систему Addressables в указанную группу и назначает ему адрес.
        /// </summary>
        /// <param name="path">Путь к ассету в проекте.</param>
        /// <param name="address">Уникальный адрес ассета.</param>
        /// <param name="groupName">Имя группы (например, "EquipmentIcons").</param>
        /// <exception cref="ArgumentNullException">Генерируется, если критически важные данные отсутствуют.</exception>
        public static void RegisterInAddressables(string path, string address, string groupName = "Default Local Group")
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            // Получаем GUID ассета
            string guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid))
            {
                throw new FileNotFoundException($"Не удалось получить GUID для ассета по пути: {path}");
            }

            // Поиск или создание группы
            AddressableAssetGroup targetGroup = settings.FindGroup(groupName);
            if (targetGroup == null)
            {
                // Создаем новую группу, если она не найдена. 
                // Используем схемы настроек из стандартной группы для корректного Build/Load Path.
                targetGroup = settings.CreateGroup(
                    groupName,
                    setAsDefaultGroup: false,
                    readOnly: false,
                    postEvent: true,
                    schemasToCopy: settings.DefaultGroup.Schemas);
            }

            // Создаем запись или перемещаем её в нужную группу
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, targetGroup);

            // Назначаем адрес, если он отличается
            if (entry is not null && entry.address != address)
            {
                entry.address = address;
            }
        }

    }
}
