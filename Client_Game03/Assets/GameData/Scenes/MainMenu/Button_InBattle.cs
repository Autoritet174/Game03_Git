using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_InBattle : MonoBehaviour {
    public void OnClick() {
        SceneManager.LoadScene("BattleField");
    }
}
