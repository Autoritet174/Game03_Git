using Assets.GameData.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    /// <summary>Создаёт категории полей боя и настраивает интерфейс подготовки сражения.</summary>
    public class SelectBattlefieldSceneInitializator : MonoBehaviour
    {
        public bool initialized { get; private set; }

        public float width { get; private set; }

        public float height { get; private set; }

        public const float SCROLLVIEW_WIDTH = 32f;

        public PanelPrepareBattle panelPrepareBattle { get; private set; }

        public PanelTop__prefab__scriptMB panelTop__prefab__context { get; private set; }

        private RectTransform scrollViewCollectionMain__RectTransform;
        private RectTransform scrollbarVertical__RectTransform;
        private RectTransform viewportMain__RectTransform;
        private RectTransform contentMain__RectTransform;

        private readonly Dictionary<string, BattlefieldCategory> dictBattlefieldCategory = new();

        private void Start()
        {
            panelTop__prefab__context = GameObjectFinder.FindByName("PanelTop__prefab").GetComponent<PanelTop__prefab__scriptMB>();
            panelTop__prefab__context.Initialize();
            panelTop__prefab__context.SetActionOnButtonClose(G.ButtonCloseOnClick);

            panelPrepareBattle = new()
            {
                sceneOnResized = OnResized,
                selectBattlefieldSceneInitializator = this
            };

            // ScrollViewCollectionMain
            {
                scrollViewCollectionMain__RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollViewCollectionMain");

                scrollbarVertical__RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical", scrollViewCollectionMain__RectTransform);

                // ViewportMain
                {
                    viewportMain__RectTransform = GameObjectFinder.FindByName<RectTransform>("ViewportMain", scrollViewCollectionMain__RectTransform);

                    // ContentMain
                    {
                        contentMain__RectTransform = GameObjectFinder.FindByName<RectTransform>("ContentMain");

                        // Испытательные площадки
                        {
                            BattlefieldCategory scrollViewCollection_TestPlatforms = new("TestPlatforms", panelPrepareBattle);
                            dictBattlefieldCategory.Add(scrollViewCollection_TestPlatforms.name, scrollViewCollection_TestPlatforms);
                            scrollViewCollection_TestPlatforms.ButtonsAdd(General.EBattleFiled.TestPlatforms__Polygon);
                        }

                        // Шахты
                        {
                            BattlefieldCategory scrollViewCollection_Mines = new("Mines", panelPrepareBattle);
                            dictBattlefieldCategory.Add(scrollViewCollection_Mines.name, scrollViewCollection_Mines);
                            scrollViewCollection_Mines.ButtonsAdd(General.EBattleFiled.Mines__Iron);
                        }
                    }

                }
            }

            panelPrepareBattle.Initialize();
            _ = scrollViewCollectionMain__RectTransform.gameObject.GetComponent<PanelCollection__prefab__scriptMB>();

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

            panelTop__prefab__context.OnResized(coefHeight);

            scrollViewCollectionMain__RectTransform.sizeDelta = new(width, height - (G.PANELTOP_HEIGHT * coefHeight));

            float scrollView_Width = SCROLLVIEW_WIDTH * coefHeight;
            scrollbarVertical__RectTransform.sizeDelta = new(scrollView_Width, 0f);
            viewportMain__RectTransform.sizeDelta = new(width - scrollView_Width, 0f);
            viewportMain__RectTransform.anchoredPosition = Vector2.zero;
            contentMain__RectTransform.anchoredPosition = Vector2.zero;

            foreach (KeyValuePair<string, BattlefieldCategory> item in dictBattlefieldCategory)
            {
                item.Value.OnResize();
            }

            panelPrepareBattle.OnResized(coefHeight);
        }
    }
}
