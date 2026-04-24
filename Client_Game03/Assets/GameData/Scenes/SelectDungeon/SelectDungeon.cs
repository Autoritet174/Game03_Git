using Assets.GameData.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    GameObject dungeonButton_Polygon_GameObject;
    Image dungeonButton_Polygon_Image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dungeonButton_Polygon_GameObject = GameObjectFinder.FindByName("DungeonButton_Polygon (id=6clw4gkc)");
        dungeonButton_Polygon_Image = GameObjectFinder.FindByName<Image>("Image");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
