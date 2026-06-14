using Assets.GameData.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class SelectBattlefieldViewerContext : IPanelCollectionViewerContext
    {
        private readonly PanelCollection__prefab__scriptMB _PanelCollection;
        private readonly HashSet<Guid> _SelectedHeroIds = new();

        public SelectBattlefieldViewerContext(PanelCollection__prefab__scriptMB panelCollection)
        {
            _PanelCollection = panelCollection;
        }

        public ECollectionMode CollectionMode => ECollectionMode.Hero;

        public int PageCurrent => SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance.PanelCollection.PageCurrent;

        public int PageMax => SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance.PanelCollection.PageMax;

        public bool LoadAllPages => false;

        public bool ContextControlsRootSize => true;

        public List<Action> Actions { get; } = new List<Action>();

        public void OnCollectionLoaded(PanelCollection__prefab__scriptMB panelCollection, int maxCollectionElements)
        {
            panelCollection.UpdatePageMax();
            panelCollection.SetPageDiapason(maxCollectionElements);

            foreach (Guid heroId in _SelectedHeroIds)
            {
                panelCollection.GetElement(heroId)?.Selected(false, clearOthers: false);
            }
        }

        public Guid? GetSelectedElementId(ECollectionMode collectionMode)
        {
            return null;
        }

        public void OnElementSelected(Guid elementId, ECollectionMode collectionMode)
        {
            if (collectionMode != ECollectionMode.Hero)
            {
                return;
            }

            if (_SelectedHeroIds.Contains(elementId))
            {
                _ = _SelectedHeroIds.Remove(elementId);
                _PanelCollection.GetElement(elementId)?.Selected(false, clearOthers: false);
            }
            else
            {
                _ = _SelectedHeroIds.Add(elementId);
                _PanelCollection.GetElement(elementId)?.Selected(true, clearOthers: false);
            }

            foreach (Action a in Actions)
            {
                a();
            }
        }

        public void OnLayoutChanged()
        {
            if (SelectBattlefieldSceneInitializator.IsConfigured)
            {
                SelectBattlefieldSceneInitializator.Instance.OnResized();
            }
        }

        public (float width, float height) GetViewerSize()
        {
            PanelCollection__prefab__scriptMB panelCollection =
                SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance.PanelCollection;
            float width = panelCollection.Width;
            float height = panelCollection.Height - panelCollection.TopButtonsHeight;
            return (width, height);
        }

        public void ClearSelection()
        {
            foreach (Guid heroId in _SelectedHeroIds.ToArray())
            {
                _PanelCollection.GetElement(heroId)?.Selected(false, clearOthers: false);
            }

            _SelectedHeroIds.Clear();
        }

        public Guid[] GetSelectedHeroIds()
        {
            return _SelectedHeroIds.ToArray();
        }

    }
}
