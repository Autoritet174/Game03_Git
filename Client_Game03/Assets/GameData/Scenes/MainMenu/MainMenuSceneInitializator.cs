using Assets.GameData.Scripts;
using UnityEngine;

/// <summary>Инициализирует главное меню и обновляет его раскладку.</summary>
public class MainMenuSceneInitializator : MonoBehaviour
{
    public bool initialized { get; private set; }

    public float width { get; private set; }

    public float height { get; private set; }

    public PanelTop__prefab__scriptMB panelTop__prefab__context { get; private set; }

    private void Start()
    {
        //Button ButtonClose = GameObjectFinder.FindByName<Button>("ButtonClose");
        //ButtonClose.onClick.RemoveAllListeners();
        //ButtonClose.onClick.AddListener(GameExitHandler.ExitGame);
        panelTop__prefab__context = GameObjectFinder.FindByName("PanelTop__prefab").GetComponent<PanelTop__prefab__scriptMB>();
        panelTop__prefab__context.Initialize();
        panelTop__prefab__context.SetActionOnButtonClose(G.ButtonCloseOnClick);
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        if (!Mathf.Approximately(Screen.height, height) || !Mathf.Approximately(Screen.width, width))
        {
            OnResized();
        }
    }

    private void OnResized()
    {

        height = Screen.height;
        width = Screen.width;
        float coefHeight = G.GetCoefHeight();
        panelTop__prefab__context.OnResized(coefHeight);
    }
}
