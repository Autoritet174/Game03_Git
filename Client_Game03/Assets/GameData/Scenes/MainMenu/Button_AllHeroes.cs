using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_AllHeroes : MonoBehaviour {
    public void OnClick() {
        SceneManager.LoadScene("AllHeroes");
    }
}
