using Assets.GameData.Scripts;
using System;
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
    TextMeshProUGUI buttonHeroesTmp;

    RectTransform buttonItems;
    TextMeshProUGUI buttonItemsTmp;

    void Start()
    {
        buttonHeroes = GameObjectFinder.FindByName<RectTransform>("ButtonHeroes (id=40jhb51a)");
        buttonHeroesTmp = GameObjectFinder.FindByName<TextMeshProUGUI>("Text (TMP) (id=wl92ls1m)");

        buttonItems = GameObjectFinder.FindByName<RectTransform>("ButtonItems (id=k5hqeyat)");
        buttonItemsTmp = GameObjectFinder.FindByName<TextMeshProUGUI>("Text (TMP) (id=cklw2id1)");


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
        _lastHeight = Screen.height;
        _lastWidth = Screen.width;

        buttonHeroesTmp.fontSize = 32f * _lastHeight / 1080;
        buttonItemsTmp.fontSize = 32f * _lastHeight / 1080;
    }
}
