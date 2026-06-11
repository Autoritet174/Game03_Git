using Assets.GameData.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public int PageCurrent => 1;

        public int PageMax => GetPageMax();

        public bool LoadAllPages => true;

        public bool ContextControlsRootSize => false;

        public void OnCollectionLoaded(PanelCollectionViewer__prefab__scriptMB viewer, int maxCollectionElements)
        {
        }

        public Guid? GetSelectedElementId(ECollectionMode collectionMode)
        {
            return collectionMode == ECollectionMode.Hero && _SelectedHeroIds.Count == 1
                ? _SelectedHeroIds.First()
                : null;
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
        }

        public void OnLayoutChanged()
        {
            if (SelectBattlefieldSceneInitializator.Instance != null)
            {
                SelectBattlefieldSceneInitializator.Instance.OnResized();
            }
        }

        public (float width, float height) GetViewerSize()
        {
            RectTransform rectTransform = _Viewer.GetComponent<RectTransform>();
            return (rectTransform.rect.width, rectTransform.rect.height);
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

        private static int GetPageMax()
        {
            int count = Game03Client.Collection.CollectionProvider.GetCountHeroes();
            int pageSize = Game03Client.Collection.CollectionProvider.PAGE_SIZE;
            int pageMax = (count / pageSize) + (count % pageSize > 0 ? 1 : 0);
            return pageMax < 1 ? 1 : pageMax;
        }
    }
}
