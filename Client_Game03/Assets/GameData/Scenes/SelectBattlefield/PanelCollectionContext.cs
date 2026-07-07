using Assets.GameData.Prefabs;
using Assets.GameData.Scenes.Collection;
using System;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class PanelCollectionContext : IPanelCollectionContext
    {
        //private SelectBattlefieldSceneInitializator selectBattlefieldSceneInitializator;
        private PanelCollection__prefab__scriptMB panelCollection__prefab;
        public void OnCollectionLoaded(SelectBattlefieldSceneInitializator selectBattlefieldSceneInitializator)
        {
            //this.selectBattlefieldSceneInitializator = selectBattlefieldSceneInitializator;
            panelCollection__prefab = selectBattlefieldSceneInitializator.panelPrepareBattle.panelCollection__prefab;
        }

        public void OnClick(Guid elementId, ECollectionMode collectionMode)
        {
            PanelIconCollectionElement e = panelCollection__prefab.GetElement(elementId);
            e?.SetSelected(!e.selected, false);
        }
    }
}
