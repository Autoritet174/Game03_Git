using Assets.GameData.Scripts;
using UnityEngine;

namespace Assets.GameData.Scenes.SelectBattlefield
{
    public class SelectBattlefieldCollectionContext
    {
        private const float PANEL_COLLECTION_WIDTH_RATIO = 0.666f;

        public bool ContextControlsRootSize => true;

        public (float width, float height) GetPanelSize()
        {
            float coefHeight = G.GetCoefHeight();
            float height = Screen.height - (G.PANELTOP_HEIGHT * coefHeight);
            float width = Screen.width * PANEL_COLLECTION_WIDTH_RATIO;
            return (width, height);
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
