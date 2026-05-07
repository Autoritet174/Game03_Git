using Assets.GameData.Scripts;
using General;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class BattleFieldCategory
    {
        private const float TEXTNAME_HEIGHT = 50;
        private const float TEXTNAME_FONTSIZE = 30;
        //private const float CONTENT_MAIN_WIDTH = 1841.7f;

        private const float CONTENT_LEFTRIGHTBOTTOM = 20;
        private const float CONTENT_BOTTOMADDITIONAL = 9;
        private const float CONTENT_TOP = CONTENT_LEFTRIGHTBOTTOM * 3;
        private const float CONTENT_SPACING = CONTENT_LEFTRIGHTBOTTOM / 2;

        private const float DUNGEON_BUTTON_HEIGHT = 121.7602f;

        private readonly Dictionary<General.EBattleFiled, BattleFieldButton> buttons = new();

        public string Name { get; }
        public string LocalizationKey { get; }
        private readonly GameObject gameObject;
        public RectTransform rectTransform { get; }

        private readonly GridLayoutGroup contentBlock__GridLayoutGroup;

        private readonly TextMeshProUGUI textName__TextMeshProUGUI;
        public BattleFieldCategory(string name)
        {
            Name = name;
            gameObject = GameObjectFinder.FindByName($"ScrollViewCollection_{name}");
            rectTransform = gameObject.GetComponent<RectTransform>();
            textName__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("TextName", gameObject.transform);
            contentBlock__GridLayoutGroup = GameObjectFinder.FindByName<GridLayoutGroup>("ContentBlock", gameObject.transform);
            LocalizationKey = $"{L.UI.Label.BattleField}_{name}";
            textName__TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue(LocalizationKey));
        }

        public void ButtonsClear()
        {
            buttons.Clear();
        }

        public void ButtonsAdd(General.EBattleFiled name)
        {
            BattleFieldButton button = new(name, this);
            buttons.Add(button.Name, button);
        }

        public void OnResize()
        {
            float coefHeight = G.GetCoefHeight();


            // Текст
            textName__TextMeshProUGUI.rectTransform.anchoredPosition = new Vector2(CONTENT_LEFTRIGHTBOTTOM * coefHeight, 0f);
            textName__TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(0f, TEXTNAME_HEIGHT * coefHeight);
            textName__TextMeshProUGUI.fontSize = TEXTNAME_FONTSIZE * coefHeight;


            // dungeonButton
            float dungeonButtonHeight = DUNGEON_BUTTON_HEIGHT * coefHeight;
            float dungeonButtonWidth = dungeonButtonHeight / 0.5625f;

            float ScrollView_Width = SelectDungeonScene.SCROLLVIEW_WIDTH * coefHeight;
            float contentSpacing = CONTENT_SPACING * coefHeight;

            // Ширина для кнопок после первой
            float delta1 = Screen.width - ScrollView_Width - (CONTENT_LEFTRIGHTBOTTOM * 2 * coefHeight) - dungeonButtonWidth;
            int countDungeonButtonInRow = ((int)(delta1 / (dungeonButtonWidth + contentSpacing))) + 1;


            // GridLayoutGroup
            int lrb = (int)(CONTENT_LEFTRIGHTBOTTOM * coefHeight);
            contentBlock__GridLayoutGroup.padding.left = lrb;
            contentBlock__GridLayoutGroup.padding.right = lrb;
            contentBlock__GridLayoutGroup.padding.bottom = lrb;
            contentBlock__GridLayoutGroup.padding.top = (int)(CONTENT_TOP * coefHeight);
            contentBlock__GridLayoutGroup.spacing = new Vector2(contentSpacing, contentSpacing);
            contentBlock__GridLayoutGroup.cellSize = new Vector2(dungeonButtonWidth, dungeonButtonHeight);
            contentBlock__GridLayoutGroup.constraintCount = countDungeonButtonInRow > 1 ? countDungeonButtonInRow : 1;


            int elementCount = buttons.Count;
            int rowCount = (elementCount / countDungeonButtonInRow) + (elementCount % countDungeonButtonInRow == 0 ? 0 : 1);
            if (rowCount < 1)
            {
                rowCount = 1;
            }

            rectTransform.sizeDelta = new Vector2(
                Screen.width - ScrollView_Width,
                (dungeonButtonHeight * rowCount) + ((CONTENT_LEFTRIGHTBOTTOM + CONTENT_TOP + CONTENT_BOTTOMADDITIONAL + (CONTENT_SPACING * (rowCount - 1))) * coefHeight)
                );


            foreach (KeyValuePair<EBattleFiled, BattleFieldButton> item in buttons)
            {
                item.Value.OnResize(dungeonButtonWidth, dungeonButtonHeight);
            }
        }
    }
}
