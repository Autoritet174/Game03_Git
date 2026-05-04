using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class BattleFieldButton
    {
        private const float LABEL_HEIGHT = 27;
        private const float LABEL_FONTSIZE = 22;
        private const float IMAGE_SELECTED_SIZE = 10;

        private readonly BattleFieldCategory parentBattleFieldCategory;
        private readonly Transform parentTransform;
        private readonly RectTransform imageMask__RectTransform;
        private readonly RectTransform image__RectTransform;
        private readonly GameObject imageSelectedMask__GameObject;
        private readonly RectTransform imageSelectedMask__RectTransform;
        private readonly RectTransform imageSelected__RectTransform;
        private readonly TextMeshProUGUI label__TextMeshProUGUI;

        public string Name { get; }

        public BattleFieldButton(string name, BattleFieldCategory parentBattleFieldCategory)
        {
            Name = name;
            this.parentBattleFieldCategory = parentBattleFieldCategory;
            parentTransform = parentBattleFieldCategory.rectTransform.transform;

            imageMask__RectTransform = GameObjectFinder.FindByName<RectTransform>("ImageMask", parentTransform);
            image__RectTransform = GameObjectFinder.FindByName<RectTransform>("Image", imageMask__RectTransform.transform);

            imageSelectedMask__RectTransform = GameObjectFinder.FindByName<RectTransform>("ImageSelectedMask", parentTransform);
            imageSelected__RectTransform = GameObjectFinder.FindByName<RectTransform>("ImageSelected", imageSelectedMask__RectTransform.transform);
            imageSelectedMask__GameObject = imageSelectedMask__RectTransform.gameObject;

            label__TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label", parentTransform);

            label__TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue($"{parentBattleFieldCategory.LocalizationKey}_{name}"));

            EventHelper.AddHoverEvents(imageMask__RectTransform.gameObject, OnPointerEnter, OnPointerExit);
            EventHelper.SetClickEvent(imageMask__RectTransform.gameObject, OnClick, false);
        }


        public void OnResize(float dungeonButtonWidth, float dungeonButtonHeight)
        {
            float coefHeight = G.GetCoefHeight();

            imageMask__RectTransform.sizeDelta = new Vector2(dungeonButtonWidth, dungeonButtonHeight);
            image__RectTransform.sizeDelta = new Vector2(dungeonButtonWidth, dungeonButtonHeight);

            float imageSelectedSize = IMAGE_SELECTED_SIZE * coefHeight;
            imageSelectedMask__RectTransform.sizeDelta = new Vector2(dungeonButtonWidth + imageSelectedSize, dungeonButtonHeight + imageSelectedSize);
            imageSelected__RectTransform.sizeDelta = new Vector2(dungeonButtonWidth + imageSelectedSize, dungeonButtonHeight + imageSelectedSize);

            label__TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(0f, LABEL_HEIGHT * coefHeight);
            label__TextMeshProUGUI.fontSize = LABEL_FONTSIZE * coefHeight;
        }

        private async UniTask OnPointerEnter()
        {
            imageSelectedMask__GameObject.SetActive(true);
        }

        private async UniTask OnPointerExit()
        {
            imageSelectedMask__GameObject.SetActive(false);
        }

        private async UniTask OnClick()
        {
            Debug.Log($"click {Name}");
        }

    }
}
