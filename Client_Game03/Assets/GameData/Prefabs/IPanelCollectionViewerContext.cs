using System;
using System.Collections.Generic;

namespace Assets.GameData.Prefabs
{
    public interface IPanelCollectionViewerContext
    {
        ECollectionMode CollectionMode { get; }
        int PageCurrent { get; }
        int PageMax { get; }
        void OnCollectionLoaded(PanelCollection__prefab__scriptMB panelCollection, int maxCollectionElements);
        Guid? GetSelectedElementId(ECollectionMode collectionMode);
        void OnElementSelected(Guid elementId, ECollectionMode collectionMode);
        void OnLayoutChanged();
        bool LoadAllPages { get; }
        bool ContextControlsRootSize { get; }
        (float width, float height) GetViewerSize();
        List<Action> Actions { get; }
    }
}
