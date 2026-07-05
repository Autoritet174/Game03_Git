//using Assets.GameData.Prefabs;
//using Cysharp.Threading.Tasks;

//namespace Assets.GameData.Scenes.SelectBattlefield
//{
//    public class SelectBattlefieldTopButtonsContext
//    {
//        public ECollectionMode CollectionMode => ECollectionMode.Hero;

//        public bool ContextControlsRootSize => true;

//        public int GetCollectionCount(ECollectionMode collectionMode)
//        {
//            return collectionMode == ECollectionMode.Hero
//                ? Game03Client.Collection.CollectionProvider.GetCountHeroes()
//                : 0;
//        }

//        public float GetPanelWidth()
//        {
//            return SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance.PanelCollection__prefab.Width;
//        }

//        //public async UniTask OnPageChangedAsync(int pageCurrent)
//        //{
//        //    await SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance.PanelCollection__prefab
//        //        .InstantiateCollection(ECollectionMode.Hero);
//        //}

//        public void OnLayoutChanged()
//        {
//            if (SelectBattlefieldSceneInitializator.IsConfigured
//                && SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance != null)
//            {
//                SelectBattlefieldSceneInitializator.PanelPrepareBattleInstance.OnResized();
//            }
//        }
//    }
//}
