#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.GameData.Editor
{
    public static class PanelCollectionPrefabCreator
    {
        private const string PREFAB_PATH = "Assets/GameData/Prefabs/PanelCollection__prefab.prefab";
        private const string SCENE_PATH = "Assets/GameData/Scenes/Collection/CollectionScene.unity";
        private const string SCENE_OBJECT_NAME = "PanelCollection (id=jcxwa01g)";

        [InitializeOnLoadMethod]
        private static void CreatePrefabOnLoadIfNeeded()
        {
            EditorApplication.delayCall += TryCreatePrefab;
        }

        [MenuItem("Tools/Game03/Create PanelCollection Prefab")]
        public static void CreateFromMenu()
        {
            TryCreatePrefab(force: true);
        }

        private static void TryCreatePrefab()
        {
            TryCreatePrefab(force: false);
        }

        private static void TryCreatePrefab(bool force)
        {
            if (!force && AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH) != null)
            {
                return;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            string activeScenePath = EditorSceneManager.GetActiveScene().path;
            bool openedTemporarily = false;

            if (activeScenePath != SCENE_PATH)
            {
                if (!System.IO.File.Exists(SCENE_PATH))
                {
                    return;
                }

                EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Single);
                openedTemporarily = true;
            }

            GameObject sourceObject = FindSceneObject(SCENE_OBJECT_NAME);
            if (sourceObject == null)
            {
                if (openedTemporarily && !string.IsNullOrEmpty(activeScenePath))
                {
                    EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
                }

                return;
            }

            if (sourceObject.GetComponent<PanelCollection__prefab__scriptMB>() == null)
            {
                sourceObject.AddComponent<PanelCollection__prefab__scriptMB>();
            }

            PrefabUtility.SaveAsPrefabAssetAndConnect(
                sourceObject,
                PREFAB_PATH,
                InteractionMode.AutomatedAction);

            sourceObject.name = SCENE_OBJECT_NAME;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            if (openedTemporarily && !string.IsNullOrEmpty(activeScenePath))
            {
                EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Created prefab: {PREFAB_PATH}");
        }

        private static GameObject FindSceneObject(string objectName)
        {
            return Resources.FindObjectsOfTypeAll<GameObject>()
                .FirstOrDefault(go => go.name == objectName && go.scene.path == SCENE_PATH && !EditorUtility.IsPersistent(go));
        }
    }
}
#endif
