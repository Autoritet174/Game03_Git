using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class StaticRemover : EditorWindow
{
    [MenuItem("Tools/Remove Static From Everything")]
    public static void ShowWindow()
    {
        GetWindow<StaticRemover>("Удаление Static");
    }

    void OnGUI()
    {
        GUILayout.Label("Удаление Static флагов", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Это действие:\n" +
            "1. Снимет Static со всех объектов на всех сценах\n" +
            "2. Снимет Static со всех префабов в проекте\n" +
            "3. Действие НЕОБРАТИМО!\n" +
            "4. Рекомендую сделать бэкап проекта!",
            MessageType.Warning
        );

        GUILayout.Space(20);

        if (GUILayout.Button("Снять Static со ВСЕГО", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog("Подтверждение",
                "Вы уверены, что хотите снять Static со всех объектов и префабов?\n\n" +
                "Это действие нельзя отменить!", "Да, снять Static", "Отмена"))
            {
                RemoveAllStaticFlags();
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Только сцены (без префабов)", GUILayout.Height(30)))
        {
            RemoveStaticFromAllScenes();
        }

        if (GUILayout.Button("Только префабы (без сцен)", GUILayout.Height(30)))
        {
            RemoveStaticFromAllPrefabs();
        }
    }

    // Главный метод - снимает со всего
    static void RemoveAllStaticFlags()
    {
        RemoveStaticFromAllScenes();
        RemoveStaticFromAllPrefabs();

        Debug.Log("✅ Static успешно удален со всех объектов и префабов!");
        EditorUtility.DisplayDialog("Готово!",
            "Static удален со всех объектов на сценах и во всех префабах!", "OK");
    }

    // Снимает Static со всех объектов на всех сценах
    static void RemoveStaticFromAllScenes()
    {
        // Сохраняем текущую сцену
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

        // Находим все сцены в проекте
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");

        int totalObjectsProcessed = 0;

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(scenePath);

            // Открываем сцену
            if (!scene.isLoaded)
            {
                scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
            }

            // Получаем все корневые объекты
            GameObject[] rootObjects = scene.GetRootGameObjects();
            int objectsInScene = 0;

            foreach (GameObject obj in rootObjects)
            {
                objectsInScene += RemoveStaticFromGameObjectAndChildren(obj);
            }

            totalObjectsProcessed += objectsInScene;
            Debug.Log($"Сцена {Path.GetFileName(scenePath)}: обработано {objectsInScene} объектов");

            // Сохраняем сцену
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        }

        // Возвращаемся к исходной сцене
        if (!string.IsNullOrEmpty(currentScene))
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(currentScene);
        }

        Debug.Log($"✅ Всего обработано {totalObjectsProcessed} объектов на всех сценах");
    }

    // Снимает Static со всех префабов в проекте
    static void RemoveStaticFromAllPrefabs()
    {
        // Находим все префабы
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int totalPrefabsProcessed = 0;

        foreach (string guid in prefabGuids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                int objectsInPrefab = RemoveStaticFromGameObjectAndChildren(prefab);
                totalPrefabsProcessed += objectsInPrefab;

                // Сохраняем изменения в префабе
                EditorUtility.SetDirty(prefab);
                AssetDatabase.SaveAssetIfDirty(prefab);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"✅ Всего обработано {totalPrefabsProcessed} объектов в префабах");
    }

    // Рекурсивно снимает Static с GameObject и всех его детей
    static int RemoveStaticFromGameObjectAndChildren(GameObject obj)
    {
        int count = 0;

        // Снимаем Static с текущего объекта
        if (obj != null)
        {
            GameObjectUtility.SetStaticEditorFlags(obj, 0); // Снимаем все Static флаги
            count++;
        }

        // Рекурсивно обрабатываем всех детей
        foreach (Transform child in obj.transform)
        {
            count += RemoveStaticFromGameObjectAndChildren(child.gameObject);
        }

        return count;
    }
}
