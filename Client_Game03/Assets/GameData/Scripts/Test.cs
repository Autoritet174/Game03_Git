using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    /// <summary>
    /// Панель содержимого Scroll View.
    /// </summary>
    public Transform contentPanel;

    /// <summary>
    /// Префаб квадрата.
    /// </summary>
    public GameObject squarePrefab;

    /// <summary>
    /// Массив текстур для квадратов.
    /// </summary>
    public Sprite[] textures;

    /// <summary>
    /// Добавляет квадраты в Scroll View при запуске.
    /// </summary>
    void Start() {
        for (int i = 0; i < textures.Length; i++) {
            AddSquare(textures[i]);
        }
    }

    /// <summary>
    /// Добавляет один квадрат с заданной текстурой.
    /// </summary>
    /// <param name="texture">Текстура для квадрата.</param>
    void AddSquare(Sprite texture) {
        GameObject square = Instantiate(squarePrefab, contentPanel);
        Image image = square.GetComponent<Image>();
        if (image != null) {
            image.sprite = texture;
        }
    }
}
