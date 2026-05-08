using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class SelectBattlefieldSceneInitializator : MonoBehaviour
    {
        public const float SCROLLVIEW_WIDTH = 32f;

        private RectTransform ScrollViewCollectionMain__RectTransform;
        private RectTransform PanelPrepareBattle__RectTransform;

        private RectTransform ScrollbarVertical__RectTransform;
        private RectTransform ViewportMain__RectTransform;
        private RectTransform ContentMain__RectTransform;

        private readonly Dictionary<string, BattleFieldCategory> dictBattleFieldCategory = new();

        private float _Width, _Height;
        private bool initialized = false;

        private void Start()
        {
            ScrollbarVertical__RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical (id=gez98o51)");
            ViewportMain__RectTransform = GameObjectFinder.FindByName<RectTransform>("ViewportMain (id=sno6hebj)");

            ScrollViewCollectionMain__RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollViewCollectionMain (id=elrwytkp)");
            PanelPrepareBattle__RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelPrepareBattle (id=373scb8n)");
            ContentMain__RectTransform = GameObjectFinder.FindByName<RectTransform>("ContentMain (id=ieb9sss4)");


            // Испытательные площадки
            {
                BattleFieldCategory scrollViewCollection_TestPlatforms = new("TestPlatforms");
                dictBattleFieldCategory.Add(scrollViewCollection_TestPlatforms.Name, scrollViewCollection_TestPlatforms);

                scrollViewCollection_TestPlatforms.ButtonsAdd(General.EBattleFiled.TestPlatforms__Polygon);
            }


            // Шахты
            {
                BattleFieldCategory scrollViewCollection_Mines = new("Mines");
                dictBattleFieldCategory.Add(scrollViewCollection_Mines.Name, scrollViewCollection_Mines);

                scrollViewCollection_Mines.ButtonsAdd(General.EBattleFiled.Mines__Iron);
            }

            //EventHelper.SetClickEvent(dungeonButton_Polygon_GameObject, DungeonButtonPolygonOnClick, false);
            //EventHelper.AddHoverEvents(_GameObject, OnPointerEnter, OnPointerExit);

            PanelPrepareBattle__RectTransform.gameObject.SetActive(false);
            PanelPrepareBattle__RectTransform.anchoredPosition = Vector3.zero;

            initialized = true;
            OnResized();
        }

        private async UniTask BattlefieldButtonPolygonOnClick()
        {

        }

        private void Update()
        {
            if (!Mathf.Approximately(Screen.height, _Height) || !Mathf.Approximately(Screen.width, _Width))
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

            _Height = Screen.height;
            _Width = Screen.width;

            float coefHeight = G.GetCoefHeight();

            ScrollViewCollectionMain__RectTransform.sizeDelta = new Vector2(_Width, (1080f - G.PANELTOP_HEIGHT) * coefHeight);

            float ScrollView_Width = SCROLLVIEW_WIDTH * coefHeight;
            ScrollbarVertical__RectTransform.sizeDelta = new Vector2(ScrollView_Width, 0f);
            ViewportMain__RectTransform.sizeDelta = new Vector2(_Width - ScrollView_Width, 0f);
            ViewportMain__RectTransform.anchoredPosition = Vector2.zero;

            ContentMain__RectTransform.anchoredPosition = Vector2.zero;

            foreach (KeyValuePair<string, BattleFieldCategory> item in dictBattleFieldCategory)
            {
                item.Value.OnResize();
            }
        }
    }
}
