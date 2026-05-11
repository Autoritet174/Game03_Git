using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using L = General.LocalizationKeys;
using I = CollectionSceneInitializator;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelTop
    {
        public PanelTop()
        {
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelTop (id=ibal8ya0)");
            _ButtonClose_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose");

            _TabButtonHeroes = new("ButtonHeroes (id=40jhb51a)", "Text (TMP) (id=wl92ls1m)", OnClickHeroes);
            _TabButtonHeroes.SetText($"{Game03Client.LocalizationManager.GetValue(L.UI.Button.Heroes)}\r\n{Game03Client.Collection.CollectionProvider.GetCountHeroes()}");

            _TabButtonEquipment = new("ButtonEquipment (id=k5hqeyat)", "Text (TMP) (id=cklw2id1)", OnClickEquipment);
            _TabButtonEquipment.SetText($"{Game03Client.LocalizationManager.GetValue(L.UI.Button.Equipment)}\r\n{Game03Client.Collection.CollectionProvider.GetCountEquipments()}");

            //_TabButtonChangingEquipment = new("ButtonChangingEquipment (id=4r13hk1v)", "Text (TMP) (id=nzouq7ws)", OnClickChangingEquipment);
            //_TabButtonChangingEquipment.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Button.ChangingEquipment));
        }

        public float Height { get; private set; }

        private const float HEIGHT_BASE = 90f;

        private readonly RectTransform _RectTransform;
        private readonly RectTransform _ButtonClose_RectTransform;
        private readonly TabButton _TabButtonHeroes, _TabButtonEquipment;//, _TabButtonChangingEquipment;

        private const int COLOR_OFF_BUTTON_RGB_VALUE = 100;
        private static Color ColorOffButton = new(
            COLOR_OFF_BUTTON_RGB_VALUE / 255f,
            COLOR_OFF_BUTTON_RGB_VALUE / 255f,
            COLOR_OFF_BUTTON_RGB_VALUE / 255f);

        public void OnResized()
        {
            float coefHeight = G.GetCoefHeight();
            Height = HEIGHT_BASE * coefHeight;
            //_RectTransform.sizeDelta = new Vector2(Screen.width, 0f);

            // Кнопки вкладок
            float tabButtonWidth = 240f * coefHeight;
            float fontSize = Height / 4f * coefHeight;

            _TabButtonHeroes.rectTransform.sizeDelta = new Vector2(tabButtonWidth, Height);
            _TabButtonHeroes.textMeshProUGUI.fontSize = fontSize;

            _TabButtonEquipment.rectTransform.sizeDelta = new Vector2(tabButtonWidth, Height);
            _TabButtonEquipment.rectTransform.anchoredPosition = new Vector2(tabButtonWidth, 0f);
            _TabButtonEquipment.textMeshProUGUI.fontSize = fontSize;

            //_TabButtonChangingEquipment.rectTransform.sizeDelta = new Vector2(tabButtonWidth, Height);
            //_TabButtonChangingEquipment.rectTransform.anchoredPosition = new Vector2(tabButtonWidth * 2f, 0f);
            //_TabButtonChangingEquipment.textMeshProUGUI.fontSize = fontSize;

            // Кнопка "Закрыть"
            //_ButtonClose_RectTransform.sizeDelta = new Vector2(Height, Height);
        }

        /// <summary> Кнопка "Герои". </summary>
        public async UniTask OnClickHeroes()
        {
            if (I.PanelSceneInstance.CollectionMode == CollectionModeEnum.Hero)
            {
                return;
            }
            I.PanelSceneInstance.CollectionMode = CollectionModeEnum.Hero;
            //I.PanelSceneInstance.PanelCollection.PanelCollectionTopButtons.UpdateActiveButtons();
            SetColorOnTabButtons(_TabButtonHeroes);
            await I.PanelCollectionViewerInstance.InstantiateCollectionAsync();
        }

        /// <summary> Кнопка "Экипировка". </summary>
        private async UniTask OnClickEquipment()
        {
            if (I.PanelSceneInstance.CollectionMode == CollectionModeEnum.Equipment)
            {
                return;
            }
            I.PanelSceneInstance.CollectionMode = CollectionModeEnum.Equipment;
            //I.PanelSceneInstance.PanelCollection.PanelCollectionTopButtons.UpdateActiveButtons();
            SetColorOnTabButtons(_TabButtonEquipment);
            await I.PanelCollectionViewerInstance.InstantiateCollectionAsync();
        }

        /// <summary> Кнопка "Смена экипировки". </summary>
        //private async UniTask OnClickChangingEquipment()
        //{
        //    if (I.PanelSceneInstance.CollectionMode == CollectionModeEnum.ChangingEquipment)
        //    {
        //        return;
        //    }
        //    I.PanelSceneInstance.CollectionMode = CollectionModeEnum.ChangingEquipment;
        //    //SetColorOnTabButtons(_TabButtonChangingEquipment);
        //    I.PanelSceneInstance.PanelCollection.PanelCollectionTopButtons.UpdateActiveButtons();
        //    await I.PanelSceneInstance.PanelCollection.PanelCollectionViewer.InstantiateCollectionAsync();
        //}

        private void SetColorOnTabButtons(TabButton tabButtonPressed)
        {
            var array = new TabButton[] { _TabButtonHeroes, _TabButtonEquipment//, _TabButtonChangingEquipment
            };
            foreach (TabButton item in array)
            {
                item.image.color = tabButtonPressed.name == item.name ? Color.white : ColorOffButton;
            }
        }
    }
}
