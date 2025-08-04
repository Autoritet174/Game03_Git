using Assets.GameData.Scripts;
using UnityEditor;
using UnityEngine;

public class GameMessageButtonClose : MonoBehaviour
{
    [SerializeField]
    GameObject prefab_Object;
    public void OnClick() {
        if (prefab_Object != null)
        {
            Destroy(prefab_Object);
        }
    }
}
