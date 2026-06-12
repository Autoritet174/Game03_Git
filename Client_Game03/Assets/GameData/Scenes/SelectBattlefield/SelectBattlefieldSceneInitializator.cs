using Assets.GameData.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class SelectBattlefieldSceneInitializator : MonoBehaviour
    {
        public const float SCROLLVIEW_WIDTH = 32f;

        public static SelectBattlefieldSceneInitializator Instance { get; private set; }
        public static PanelPrepareBattle PanelPrepareBattleInstance { get; private set; }
        public static bool IsConfigured { get; private set; }

        private RectTransform ScrollViewCollectionMain__RectTransform;
        private RectTransform ScrollbarVertical__RectTransform;
        private RectTransform ViewportMain__RectTransform;
        private RectTransform ContentMain__RectTransform;

        private readonly Dictionary<string, BattlefieldCategory> dictBattlefieldCategory = new();

        private float _Width, _Height;

        private void Awake()
        {
            Instance = this;
            IsConfigured = false;
        }

        private void Start()
        {
            try
            {
                ScrollbarVertical__RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical (id=gez98o51)");
                ViewportMain__RectTransform = GameObjectFinder.FindByName<RectTransform>("ViewportMain (id=sno6hebj)");
                ScrollViewCollectionMain__RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollViewCollectionMain (id=elrwytkp)");
                ContentMain__RectTransform = GameObjectFinder.FindByName<RectTransform>("ContentMain (id=ieb9sss4)");

                PanelPrepareBattleInstance = new PanelPrepareBattle();

                // Испытательные площадки
                {
                    BattlefieldCategory scrollViewCollection_TestPlatforms = new("TestPlatforms");
                    dictBattlefieldCategory.Add(scrollViewCollection_TestPlatforms.Name, scrollViewCollection_TestPlatforms);
                    scrollViewCollection_TestPlatforms.ButtonsAdd(General.EBattleFiled.TestPlatforms__Polygon);
                }

                // Шахты
                {
                    BattlefieldCategory scrollViewCollection_Mines = new("Mines");
                    dictBattlefieldCategory.Add(scrollViewCollection_Mines.Name, scrollViewCollection_Mines);
                    scrollViewCollection_Mines.ButtonsAdd(General.EBattleFiled.Mines__Iron);
                }

                IsConfigured = true;
                OnResized();
            }
            catch (Exception ex)
            {
                Debug.LogError($"SelectBattlefieldSceneInitializator: scene configuration failed. {ex.Message}");
            }
        }

        private void Update()
        {
            if (!IsConfigured)
            {
                return;
            }

            if (!Mathf.Approximately(Screen.height, _Height) || !Mathf.Approximately(Screen.width, _Width))
            {
                OnResized();
            }
        }

        public void OnResized()
        {
            if (!IsConfigured)
            {
                return;
            }

            _Height = Screen.height;
            _Width = Screen.width;

            float coefHeight = G.GetCoefHeight();

            ScrollViewCollectionMain__RectTransform.sizeDelta = new Vector2(_Width, (_Height - (G.PANELTOP_HEIGHT * coefHeight)));

            float ScrollView_Width = SCROLLVIEW_WIDTH * coefHeight;
            ScrollbarVertical__RectTransform.sizeDelta = new Vector2(ScrollView_Width, 0f);
            ViewportMain__RectTransform.sizeDelta = new Vector2(_Width - ScrollView_Width, 0f);
            ViewportMain__RectTransform.anchoredPosition = Vector2.zero;
            ContentMain__RectTransform.anchoredPosition = Vector2.zero;

            foreach (KeyValuePair<string, BattlefieldCategory> item in dictBattlefieldCategory)
            {
                item.Value.OnResize();
            }

            if (PanelPrepareBattleInstance != null)
            {
                PanelPrepareBattleInstance.OnResized();
            }
        }
    }
}
