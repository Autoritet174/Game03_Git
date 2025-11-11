using Assets.GameData.Scripts;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Collection : MonoBehaviour
{

    private const int ColorOffButtonRGBValue = 100;
    private static Color ColorOffButton = new(ColorOffButtonRGBValue / 255f, ColorOffButtonRGBValue / 255f, ColorOffButtonRGBValue / 255f);
    private bool inited = false;
    private float _lastHeight;
    private float _lastWidth;

    private class TabButton
    {
        public readonly string name;
        public readonly RectTransform rectTransform;
        public readonly Button button;
        public readonly TextMeshProUGUI textMeshProUGUI;
        public readonly Image image;

        public TabButton(string name, string nameText, UnityAction action)
        {
            this.name = name;
            button = GameObjectFinder.FindByName<Button>(name);
            rectTransform = GameObjectFinder.FindByName<RectTransform>(name);
            image = GameObjectFinder.FindByName<Image>(name);
            textMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>(nameText);
            button.onClick.AddListener(action);
        }
    }

    private TabButton tabButtonHeroes, tabButtonItem;

    private readonly Dictionary<string, TabButton> tabButtonsDict = new();

    private readonly CollectionHero collectionHero = new();
    private async void Start()
    {
        tabButtonHeroes = new("ButtonHeroes (id=40jhb51a)", "Text (TMP) (id=wl92ls1m)", OnClickHeroes);
        tabButtonItem = new("ButtonItems (id=k5hqeyat)", "Text (TMP) (id=cklw2id1)", OnClickItems);

        tabButtonsDict.Add(tabButtonHeroes.button.name, tabButtonHeroes);
        tabButtonsDict.Add(tabButtonItem.button.name, tabButtonItem);

        inited = true;
        OnResize();

        //получить инвентарь героев
        Game03Client.HttpRequester.HttpRequesterResult httpRequesterProviderResult = await GlobalFields.ClientGame.HttpRequesterProvider.GetResponceAsync(General.Url.Inventory.Heroes);
        if (httpRequesterProviderResult != null)
        {
            JObject jObject = httpRequesterProviderResult.JObject;
            if (jObject != null)
            {
                JToken result = jObject["result"];
                foreach (JToken jToken in result)
                {
                    Debug.Log($"{jToken["_id"]} | {jToken["o"]} | {jToken["h"]}");
                }

            }
        }
        //string s = jObject["token"]?.ToString() ?? string.Empty;
    }

    private void Update()
    {
        if (inited && (!Mathf.Approximately(Screen.height, _lastHeight) || !Mathf.Approximately(Screen.width, _lastWidth)))
        {
            OnResize();
        }
    }

    private void OnResize()
    {
        _lastHeight = Screen.height;
        _lastWidth = Screen.width;

        //buttonHeroesTmp.fontSize = 32f * _lastHeight / 1080;
        //buttonItemsTmp.fontSize = 32f * _lastHeight / 1080;
    }

    public void OnClickHeroes()
    {
        OnClickTabButton(tabButtonHeroes);
        tabButtonHeroes.image.color = Color.white;

    }

    public void OnClickItems()
    {
        OnClickTabButton(tabButtonItem);


    }

    private void OnClickTabButton(TabButton tabButtonPressed)
    {
        foreach (KeyValuePair<string, TabButton> item in tabButtonsDict)
        {
            item.Value.image.color = item.Value.name == tabButtonPressed.name ? Color.white : ColorOffButton;
        }
    }

}
