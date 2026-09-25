using Assets.GameData.Prefabs;
using Assets.GameData.Scenes.Collection;
using System;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    /// <summary>Связывает выбор элементов коллекции с панелями текущей сцены.</summary>
    public class PanelCollectionContext : IPanelCollectionContext
    {
        //private SelectBattlefieldSceneInitializator selectBattlefieldSceneInitializator;
        private PanelCollection__prefab__scriptMB panelCollection__prefab;
        private Action updateHeroesSelectedAndMaxLabel;
        public void OnCollectionLoaded(SelectBattlefieldSceneInitializator selectBattlefieldSceneInitializator
            , Action updateHeroesSelectedAndMaxLabel)
        {
            this.updateHeroesSelectedAndMaxLabel = updateHeroesSelectedAndMaxLabel;
            panelCollection__prefab = selectBattlefieldSceneInitializator.panelPrepareBattle.panelCollection__prefab;
        }

        public void OnClick(Guid elementId, ECollectionMode collectionMode)
        {
            PanelIconCollectionElement e = panelCollection__prefab.GetElement(elementId);
            e?.SetSelected(!e.selected, false);
            updateHeroesSelectedAndMaxLabel();
        }
    }
}
