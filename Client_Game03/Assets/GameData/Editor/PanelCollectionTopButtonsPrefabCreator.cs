#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.GameData.Editor
{
    public static class PanelCollectionTopButtonsPrefabCreator
    {
        private const string PrefabPath = "Assets/GameData/Prefabs/PanelCollectionTopButtons__prefab.prefab";
        private const string ScenePath = "Assets/GameData/Scenes/Collection/CollectionScene.unity";
        private const string SceneObjectName = "PanelCollectionTopButtons (id=gmzb0h9f)";

        [InitializeOnLoadMethod]
        private static void CreatePrefabOnLoadIfNeeded()
        {
            EditorApplication.delayCall += TryCreatePrefab;
        }

        [MenuItem("Tools/Game03/Create PanelCollectionTopButtons Prefab")]
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
            if (!force && AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null)
            {
                return;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            string activeScenePath = EditorSceneManager.GetActiveScene().path;
            bool openedTemporarily = false;

            if (activeScenePath != ScenePath)
            {
                if (!System.IO.File.Exists(ScenePath))
                {
                    return;
                }

                EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
                openedTemporarily = true;
            }

            GameObject sourceObject = FindSceneObject(SceneObjectName);
            if (sourceObject == null)
            {
                if (openedTemporarily && !string.IsNullOrEmpty(activeScenePath))
                {
                    EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
                }

                return;
            }

            if (sourceObject.GetComponent<PanelCollectionTopButtons__prefab__scriptMB>() == null)
            {
                sourceObject.AddComponent<PanelCollectionTopButtons__prefab__scriptMB>();
            }

            PrefabUtility.SaveAsPrefabAssetAndConnect(
                sourceObject,
                PrefabPath,
                InteractionMode.AutomatedAction);

            sourceObject.name = SceneObjectName;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            if (openedTemporarily && !string.IsNullOrEmpty(activeScenePath))
            {
                EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Created prefab: {PrefabPath}");
        }

        private static GameObject FindSceneObject(string objectName)
        {
            return Resources.FindObjectsOfTypeAll<GameObject>()
                .FirstOrDefault(go => go.name == objectName && go.scene.path == ScenePath && !EditorUtility.IsPersistent(go));
        }
    }
}
#endif
