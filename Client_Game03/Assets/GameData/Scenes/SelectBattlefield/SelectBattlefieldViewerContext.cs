using Assets.GameData.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class SelectBattlefieldViewerContext : IPanelCollectionViewerContext
    {
        private readonly PanelCollectionViewer__prefab__scriptMB _Viewer;
        private readonly HashSet<Guid> _SelectedHeroIds = new();

        public SelectBattlefieldViewerContext(PanelCollectionViewer__prefab__scriptMB viewer)
        {
            _Viewer = viewer;
        }

        public ECollectionMode CollectionMode => ECollectionMode.Hero;

        public int PageCurrent => SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance.TopButtons.PageCurrent;

        public int PageMax => SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance.TopButtons.PageMax;

        public bool LoadAllPages => false;

        public bool ContextControlsRootSize => true;

        public List<Action> Actions { get; } = new List<Action>();

        public void OnCollectionLoaded(PanelCollectionViewer__prefab__scriptMB viewer, int maxCollectionElements)
        {
            PanelCollectionTopButtons__prefab__scriptMB topButtons =
                SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance.TopButtons;
            topButtons.UpdatePageMax();
            topButtons.SetPageDiapason(maxCollectionElements);

            foreach (Guid heroId in _SelectedHeroIds)
            {
                viewer.GetElement(heroId)?.Selected(false, clearOthers: false);
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
                _Viewer.GetElement(elementId)?.Selected(false, clearOthers: false);
            }
            else
            {
                _ = _SelectedHeroIds.Add(elementId);
                _Viewer.GetElement(elementId)?.Selected(true, clearOthers: false);
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
            PanelPrepareBattle panelPrepareBattle = SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance;
            float width = panelPrepareBattle.PanelCollection.Width;
            float height = panelPrepareBattle.PanelCollection.Height - panelPrepareBattle.TopButtons.Height;
            return (width, height);
        }

        public void ClearSelection()
        {
            foreach (Guid heroId in _SelectedHeroIds.ToArray())
            {
                _Viewer.GetElement(heroId)?.Selected(false, clearOthers: false);
            }

            _SelectedHeroIds.Clear();
        }

        public Guid[] GetSelectedHeroIds()
        {
            return _SelectedHeroIds.ToArray();
        }

    }
}
