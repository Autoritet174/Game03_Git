using Assets.GameData.Scripts;
using TMPro;
using UnityEngine;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class BattleFieldButton
    {
        private const float LABEL_HEIGHT = 27;
        private const float LABEL_FONTSIZE = 22;

        private readonly BattleFieldCategory parentBattleFieldCategory;
        private readonly Transform parentTransform;
        private readonly RectTransform imageMask__RectTransform;
        private readonly RectTransform image__RectTransform;
        private readonly TextMeshProUGUI label__TextMeshProUGUI;

        public string Name { get; }
        public BattleFieldButton(string name, BattleFieldCategory parentBattleFieldCategory)
        {
            Name = name;
            this.parentBattleFieldCategory = parentBattleFieldCategory;
            parentTransform = parentBattleFieldCategory.rectTransform.transform;
            imageMask__RectTransform = GameObjectFinder.FindByName<RectTransform>("ImageMask", parentTransform);
            label__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label", parentTransform);
            image__RectTransform = GameObjectFinder.FindByName<RectTransform>("Image", imageMask__RectTransform.transform);

            label__TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue($"{parentBattleFieldCategory.LocalizationKey}_{name}"));
        }


        public void OnResize(float dungeonButtonWidth, float dungeonButtonHeight)
        {
            float coefHeight = G.GetCoefHeight();

            imageMask__RectTransform.sizeDelta = new Vector2(dungeonButtonWidth, dungeonButtonHeight);
            image__RectTransform.sizeDelta = new Vector2(dungeonButtonWidth, dungeonButtonHeight);

            label__TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(0f, LABEL_HEIGHT * coefHeight);
            label__TextMeshProUGUI.fontSize = LABEL_FONTSIZE * coefHeight;
        }
    }
}
