using Assets.GameData.Prefabs;
using Cysharp.Threading.Tasks;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class SelectBattlefieldTopButtonsContext : IPanelCollectionTopButtonsContext
    {
        public ECollectionMode CollectionMode => ECollectionMode.Hero;

        public bool ContextControlsRootSize => true;

        public int GetCollectionCount(ECollectionMode collectionMode)
        {
            return collectionMode == ECollectionMode.Hero
                ? Game03Client.Collection.CollectionProvider.GetCountHeroes()
                : 0;
        }

        public float GetPanelWidth()
        {
            return SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance.PanelCollection.Width;
        }

        public async UniTask OnPageChangedAsync(int pageCurrent)
        {
            await SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance.Viewer
                .InstantiateCollectionAsync(ECollectionMode.Hero);
        }

        public void OnLayoutChanged()
        {
            if (SelectBattlefieldSceneInitializator.IsConfigured
                && SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance != null)
            {
                SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance.OnResized();
            }
        }
    }
}
