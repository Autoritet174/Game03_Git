using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;

public class WindowMessageInitializator : MonoBehaviour
{
    public static string PrefabTag { get; } = "WindowMessage";

    void Start()
    {
        string path = "Prefabs/L3/WindowMessage/WindowMessage-Canvas";
        GameObject prefab_Object = Resources.Load<GameObject>(path);

        prefab_Object.tag = PrefabTag;
        prefab_Object.name = PrefabTag;

        prefab_Object = Instantiate(prefab_Object);

        if (!prefab_Object.TryGetComponent(out Canvas canvas))
        {
            Destroy(prefab_Object);
            throw new System.Exception($"Not found Canvas in prefab");
        }

        if (!GameObject.FindWithTag("MainCamera").TryGetComponent(out Camera mainCamera))
        {
            throw new System.Exception($"Not found game object with tag 'MainCamera'");
        }
        canvas.worldCamera = mainCamera;

        prefab_Object.SetActive(false);
    }
}
