using System;

namespace Assets.GameData.Prefabs
{
    public interface IPanelCollectionContext
    {
        void OnClick(Guid elementId, ECollectionMode collectionMode);
    }
}
