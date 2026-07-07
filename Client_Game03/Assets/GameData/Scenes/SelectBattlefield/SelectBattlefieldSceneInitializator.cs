using Assets.GameData.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class SelectBattlefieldSceneInitializator : MonoBehaviour
    {
        public bool initialized { get; private set; }
        public float width { get; private set; }
        public float height { get; private set; }
        public const float SCROLLVIEW_WIDTH = 32f;

        public PanelPrepareBattle panelPrepareBattle { get; private set; }
        public PanelTop__prefab__scriptMB PanelTop__prefab__context { get; private set; }
        private RectTransform ScrollViewCollectionMain__RectTransform;
        private RectTransform ScrollbarVertical__RectTransform;
        private RectTransform ViewportMain__RectTransform;
        private RectTransform ContentMain__RectTransform;

        private readonly Dictionary<string, BattlefieldCategory> dictBattlefieldCategory = new();


        private void Start()
        {
            PanelTop__prefab__context = GameObjectFinder.FindByName("PanelTop__prefab").GetComponent<PanelTop__prefab__scriptMB>();

            panelPrepareBattle = new PanelPrepareBattle
            {
                SceneOnResized = OnResized,
                selectBattlefieldSceneInitializator = this
            };

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
                            BattlefieldCategory scrollViewCollection_TestPlatforms = new("TestPlatforms", panelPrepareBattle);
                            dictBattlefieldCategory.Add(scrollViewCollection_TestPlatforms.Name, scrollViewCollection_TestPlatforms);
                            scrollViewCollection_TestPlatforms.ButtonsAdd(General.EBattleFiled.TestPlatforms__Polygon);
                        }

                        // Шахты
                        {
                            BattlefieldCategory scrollViewCollection_Mines = new("Mines", panelPrepareBattle);
                            dictBattlefieldCategory.Add(scrollViewCollection_Mines.Name, scrollViewCollection_Mines);
                            scrollViewCollection_Mines.ButtonsAdd(General.EBattleFiled.Mines__Iron);
                        }
                    }

                }
            }


            
            panelPrepareBattle.Initialize();
            _ = ScrollViewCollectionMain__RectTransform.gameObject.GetComponent<PanelCollection__prefab__scriptMB>();

            initialized = true;
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
            if (!initialized)
            {
                return;
            }

            if (!Mathf.Approximately(Screen.height, height) || !Mathf.Approximately(Screen.width, width))
            {
                OnResized();
            }
        }

        public void OnResized()
        {
            if (!initialized)
            {
                return;
            }

            height = Screen.height;
            width = Screen.width;

            float coefHeight = G.GetCoefHeight();


            PanelTop__prefab__context.OnResized(coefHeight);


            ScrollViewCollectionMain__RectTransform.sizeDelta = new Vector2(width, height - (G.PANELTOP_HEIGHT * coefHeight));

            float ScrollView_Width = SCROLLVIEW_WIDTH * coefHeight;
            ScrollbarVertical__RectTransform.sizeDelta = new Vector2(ScrollView_Width, 0f);
            ViewportMain__RectTransform.sizeDelta = new Vector2(width - ScrollView_Width, 0f);
            ViewportMain__RectTransform.anchoredPosition = Vector2.zero;
            ContentMain__RectTransform.anchoredPosition = Vector2.zero;

            foreach (KeyValuePair<string, BattlefieldCategory> item in dictBattlefieldCategory)
            {
                item.Value.OnResize();
            }

            panelPrepareBattle.OnResized(coefHeight);
        }
    }
}
