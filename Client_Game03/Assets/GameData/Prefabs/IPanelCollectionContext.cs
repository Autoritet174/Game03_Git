namespace Assets.GameData.Prefabs
{
    public interface IPanelCollectionContext
    {
        bool ContextControlsRootSize { get; }
        (float width, float height) GetPanelSize();
        void OnLayoutChanged();
    }
}
