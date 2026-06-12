using Assets.GameData.Prefabs;
using Cysharp.Threading.Tasks;

namespace Assets.GameData.Scenes.Collection
{
    public class CollectionSceneTopButtonsContext : IPanelCollectionTopButtonsContext
    {
        public ECollectionMode CollectionMode => CollectionSceneInitializator.PanelSceneInstance.CollectionMode;

        public bool ContextControlsRootSize => true;

        public int GetCollectionCount(ECollectionMode collectionMode)
        {
            return collectionMode switch
            {
                ECollectionMode.Hero => Game03Client.Collection.CollectionProvider.GetCountHeroes(),
                ECollectionMode.Equipment => Game03Client.Collection.CollectionProvider.GetCountEquipments(),
                _ => 0,
            };
        }

        public float GetPanelWidth()
        {
            return CollectionSceneInitializator.PanelCollectionInstance.Width;
        }

        public async UniTask OnPageChangedAsync(int pageCurrent)
        {
            await CollectionSceneInitializator.PanelCollectionViewerInstance.InstantiateCollectionAsync(CollectionMode);
        }

        public void OnLayoutChanged()
        {
            CollectionSceneInitializator.OnResized();
        }
    }
}
