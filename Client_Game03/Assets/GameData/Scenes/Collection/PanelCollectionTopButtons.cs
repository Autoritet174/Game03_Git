using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelCollectionTopButtons
    {
        private const float HEIGHT = 113f;
        private const float RANGE_PANEL_WIDTH = 230f;
        private const float RANGE_PANEL_HEIGHT = 90f;
        private const float BUTTON_PAGE_WIDTH = 100f;
        private const float BUTTON_PAGE_HEIGHT = 60f;
        private const float LABEL_HEIGHT = 30f;
        private const float LABEL_FONTSIZE = 18f;

        public PanelCollectionTopButtons(PanelCollection panelCollection)
        {
            _PanelCollection = panelCollection;
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollectionTopButtons (id=gmzb0h9f)");

            _FilterButtonHeroes = new("ImageButtonHeroes (id=pakco5ud)");
            _FilterButtonEquipments = new("ImageButtonEquipments (id=vuhjngaz)");
            _FilterButtonFilter = new("ImageButtonFilter (id=vjeqfzen)");
            _FilterButtonGroup = new("ImageButtonGroup (id=hbsaogwl)");
            _FilterButtonSort = new("ImageButtonSort (id=6nvcsrdm)");

            _RangePanel_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelRange (id=66z5bnzi)");
            _RangePanel_GameObject = _RangePanel_RectTransform.gameObject;

            _ButtonPrevPage_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonPrevPage (id=25alql62)");
            _ButtonNextPage_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonNextPage (id=k5moi57b)");
            _ButtonPrevPage_RectTransform.gameObject.SetClickEvent(PagePrev, true);
            _ButtonNextPage_RectTransform.gameObject.SetClickEvent(PageNext, true);

            _LabelRangePage_RectTransform = GameObjectFinder.FindByName<RectTransform>("LabelRangePage (id=6jgz12bu)");
            _LabelRangePage_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("LabelRangePage (id=6jgz12bu)");
        }
        public int PageCurrent { get; private set; } = 1;
        public int PageMax { get; private set; } = 1;
        public float Height { get; private set; }

        private readonly RectTransform _RectTransform;
        private readonly PanelCollection _PanelCollection;

        private readonly FilterButton _FilterButtonHeroes;
        private readonly FilterButton _FilterButtonEquipments;
        private readonly FilterButton _FilterButtonFilter;
        private readonly FilterButton _FilterButtonGroup;
        private readonly FilterButton _FilterButtonSort;

        private readonly RectTransform _RangePanel_RectTransform;
        private readonly GameObject _RangePanel_GameObject;
        private readonly RectTransform _ButtonPrevPage_RectTransform;
        private readonly RectTransform _ButtonNextPage_RectTransform;
        private readonly RectTransform _LabelRangePage_RectTransform;
        private readonly TextMeshProUGUI _LabelRangePage_TextMeshProUGUI;

        public void UpdateActiveButtons()
        {
            switch (_PanelCollection.PanelScene.CollectionMode)
            {
                case CollectionMode.Hero:
                    _FilterButtonHeroes.SetActive(true);
                    _FilterButtonEquipments.SetActive(false);
                    break;
                case CollectionMode.Equipment:
                    _FilterButtonHeroes.SetActive(false);
                    _FilterButtonEquipments.SetActive(true);
                    break;
                case CollectionMode.ChangingEquipment:
                    //_FilterButtonHeroes.SetActive(false);
                    //_FilterButtonEquipments.SetActive(false);
                    break;
                    //throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }

        }

        public void SetPageDiapason() {
            _LabelRangePage_TextMeshProUGUI.text = $"{((PageCurrent - 1) * Game03Client.Collection.CollectionProvider.PAGE_SIZE) + 1} - {_PanelCollection.PanelCollectionViewer. MaxCollectionElements}";
        }

        public void OnResized()
        {
            float coefHeight = G.GetCoefHeight();
            Height = HEIGHT * coefHeight;
            _RectTransform.sizeDelta.Set(_PanelCollection.Width, Height);

            _FilterButtonHeroes.OnResized(0);
            _FilterButtonEquipments.OnResized(0);
            _FilterButtonFilter.OnResized(1);
            _FilterButtonGroup.OnResized(2);
            _FilterButtonSort.OnResized(3);

            float panelRangeLeft = (((FilterButton.SIZE + FilterButton.SPACING) * 4) + (FilterButton.SPACING_ADDITIONAL * 2)) * coefHeight;

            _RangePanel_RectTransform.anchoredPosition.Set(panelRangeLeft, FilterButton.SPACING * coefHeight);
            _RangePanel_RectTransform.sizeDelta.Set(RANGE_PANEL_WIDTH * coefHeight, RANGE_PANEL_HEIGHT * coefHeight);

            float buttonPageWidth = BUTTON_PAGE_WIDTH * coefHeight;
            float buttonPageHeight = BUTTON_PAGE_HEIGHT * coefHeight;
            _ButtonPrevPage_RectTransform.sizeDelta.Set(buttonPageWidth, buttonPageHeight);
            _ButtonNextPage_RectTransform.sizeDelta.Set(buttonPageWidth, buttonPageHeight);
            _LabelRangePage_RectTransform.sizeDelta.Set(RANGE_PANEL_WIDTH * coefHeight, LABEL_HEIGHT * coefHeight);
            _LabelRangePage_TextMeshProUGUI.fontSize = LABEL_FONTSIZE * coefHeight;
        }


        private async UniTask PagePrev()
        {
            if (PageCurrent > 1)
            {
                PageCurrent--;
                await _PanelCollection.PanelCollectionViewer.InstantiateCollectionAsync();
            }
        }

        private async UniTask PageNext()
        {
            if (PageCurrent < PageMax)
            {
                PageCurrent++;
                await _PanelCollection.PanelCollectionViewer.InstantiateCollectionAsync();
            }
        }

        public void UpdatePageMax()
        {
            int c = _PanelCollection.PanelScene.CollectionMode switch
            {
                CollectionMode.Hero => Game03Client.Collection.CollectionProvider.GetCountHeroes(),
                CollectionMode.Equipment => Game03Client.Collection.CollectionProvider.GetCountEquipments(),
                CollectionMode.ChangingEquipment => _PanelCollection.PanelScene.PanelSelectedHero.IsVisible
                    ? Game03Client.Collection.CollectionProvider.GetCountEquipments()
                    : Game03Client.Collection.CollectionProvider.GetCountHeroes(),
                _ => throw new Exception(),
            };
            PageMax = (c / Game03Client.Collection.CollectionProvider.PAGE_SIZE) + (c % Game03Client.Collection.CollectionProvider.PAGE_SIZE > 0 ? 1 : 0);
            if (PageMax < 1)
            {
                PageMax = 1;
            }
            if (PageCurrent > PageMax)
            {
                PageCurrent = PageMax;
            }
            _RangePanel_GameObject.SetActive(PageMax > 1);
        }
    }
}
