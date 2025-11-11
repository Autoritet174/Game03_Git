using UnityEngine;

public class GameMessageButtonClose : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab_Object;
    public void OnClick()
    {
        if (prefab_Object != null)
        {
            Destroy(prefab_Object);
        }
    }
}
