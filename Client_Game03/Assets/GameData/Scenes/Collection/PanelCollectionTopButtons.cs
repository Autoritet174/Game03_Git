using Assets.GameData.Scripts;
using System;
using UnityEngine;

namespace Assets.GameData.Scenes.Collection
{
    public class PanelCollectionTopButtons
    {
        public PanelCollectionTopButtons(PanelCollection panelCollection)
        {
            PanelCollection = panelCollection;
            _RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelCollectionTopButtons (id=gmzb0h9f)");

            _FilterButtonHeroes = new("ImageButtonHeroes (id=pakco5ud)");
            _FilterButtonEquipments = new("ImageButtonEquipments (id=vuhjngaz)");
            _FilterButtonFilter = new("ImageButtonFilter (id=vjeqfzen)");
            _FilterButtonGroup = new("ImageButtonGroup (id=hbsaogwl)");
            _FilterButtonSort = new("ImageButtonSort (id=6nvcsrdm)");
        }

        private readonly RectTransform _RectTransform;
        public PanelCollection PanelCollection { get; }

        private readonly FilterButton _FilterButtonHeroes;
        private readonly FilterButton _FilterButtonEquipments;
        private readonly FilterButton _FilterButtonFilter;
        private readonly FilterButton _FilterButtonGroup;
        private readonly FilterButton _FilterButtonSort;

        public void UpdateActiveButtons()
        {
            switch (PanelScene.CollectionMode)
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
                    //break;
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }

        }

        public void OnResized()
        {
            _RectTransform.sizeDelta = Vector2.zero;
        }
    }
}
