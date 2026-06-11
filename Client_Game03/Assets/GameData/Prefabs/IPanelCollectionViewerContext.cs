using System;

namespace Assets.GameData.Prefabs
{
    public interface IPanelCollectionViewerContext
    {
        ECollectionMode CollectionMode { get; }
        int PageCurrent { get; }
        int PageMax { get; }
        void OnCollectionLoaded(PanelCollectionViewer__prefab__scriptMB viewer, int maxCollectionElements);
        Guid? GetSelectedElementId(ECollectionMode collectionMode);
        void OnElementSelected(Guid elementId, ECollectionMode collectionMode);
        void OnLayoutChanged();
        bool LoadAllPages { get; }
        bool ContextControlsRootSize { get; }
        (float width, float height) GetViewerSize();
    }
}
