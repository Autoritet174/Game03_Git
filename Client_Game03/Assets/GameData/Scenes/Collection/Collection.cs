using Assets.GameData.Scripts;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class Collection : MonoBehaviour
{
    private bool inited = false;
    private float _lastHeight;
    private float _lastWidth;
    RectTransform buttonHeroes;
    TextMeshProUGUI button
    void Start()
    {
        buttonHeroes = GameObjectFinder.FindByName<RectTransform>("ButtonHeroes (id=40jhb51a)");


        inited = true;
        OnResize();
    }
    private void Update()
    {
        if (inited && (!Mathf.Approximately(Screen.height, _lastHeight) || !Mathf.Approximately(Screen.width, _lastWidth)))
        {
            OnResize();
        }
    }
    private void OnResize() {
        buttonHeroes.fontSize = 66.66666f * _lastHeight / 1080;
    }
}
