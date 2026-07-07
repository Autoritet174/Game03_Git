using Assets.GameData.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class SelectBattlefieldSceneInitializator : MonoBehaviour
    {
        public const float SCROLLVIEW_WIDTH = 32f;

        public PanelPrepareBattle PanelPrepareBattleInstance { get; private set; }

        private RectTransform ScrollViewCollectionMain__RectTransform;
        private RectTransform ScrollbarVertical__RectTransform;
        private RectTransform ViewportMain__RectTransform;
        private RectTransform ContentMain__RectTransform;

        private readonly Dictionary<string, BattlefieldCategory> dictBattlefieldCategory = new();

        private float Width, Height;

        private void Start()
        {
            // ScrollViewCollectionMain
            {
                ScrollViewCollectionMain__RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollViewCollectionMain");

                ScrollbarVertical__RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical", ScrollViewCollectionMain__RectTransform);

                // ViewportMain
                {
                    ViewportMain__RectTransform = GameObjectFinder.FindByName<RectTransform>("ViewportMain", ScrollViewCollectionMain__RectTransform);

                    // ContentMain
                    {
                        ContentMain__RectTransform = GameObjectFinder.FindByName<RectTransform>("ContentMain");

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
                    }

                }
            }
           


            
            

            PanelPrepareBattleInstance = new PanelPrepareBattle();
            _ = ScrollViewCollectionMain__RectTransform.gameObject.GetComponent<PanelCollection__prefab__scriptMB>();
            

            OnResized();
            //}
            //catch (Exception ex)
            //{
            //    Debug.LogError($"SelectBattlefieldSceneInitializator: scene configuration failed.");
            //    Debug.LogException(ex);
            //}
        }

        private void Update()
        {
            if (!IsConfigured)
            {
                return;
            }

            if (!Mathf.Approximately(Screen.height, Height) || !Mathf.Approximately(Screen.width, Width))
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

            Height = Screen.height;
            Width = Screen.width;

            float coefHeight = G.GetCoefHeight();

            ScrollViewCollectionMain__RectTransform.sizeDelta = new Vector2(Width, Height - (G.PANELTOP_HEIGHT * coefHeight));

            float ScrollView_Width = SCROLLVIEW_WIDTH * coefHeight;
            ScrollbarVertical__RectTransform.sizeDelta = new Vector2(ScrollView_Width, 0f);
            ViewportMain__RectTransform.sizeDelta = new Vector2(Width - ScrollView_Width, 0f);
            ViewportMain__RectTransform.anchoredPosition = Vector2.zero;
            ContentMain__RectTransform.anchoredPosition = Vector2.zero;

            foreach (KeyValuePair<string, BattlefieldCategory> item in dictBattlefieldCategory)
            {
                item.Value.OnResize();
            }

            PanelPrepareBattleInstance?.OnResized();
        }
    }
}
