using Cysharp.Threading.Tasks;

namespace Assets.GameData.Prefabs
{
    public interface IPanelCollectionTopButtonsContext
    {
        ECollectionMode CollectionMode { get; }
        int GetCollectionCount(ECollectionMode collectionMode);
        float GetPanelWidth();
        bool ContextControlsRootSize { get; }
        UniTask OnPageChangedAsync(int pageCurrent);
        void OnLayoutChanged();
    }
}
