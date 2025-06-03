using UnityEngine;
using UnityEngine.UI;

public class Content : MonoBehaviour {


    /// <summary>
    /// Метод вызывается при старте.
    /// Покрашивает все Image в Content в случайный цвет.
    /// </summary>
    private void Start() {
        ColorImagesInContent();
    }


    /// <summary>
    /// Метод для покраски всех Image в случайный цвет.
    /// </summary>
    public void ColorImagesInContent() {
        // Получаем компонент Content (ScrollView)
        Transform content = transform;

        // Проходим по всем дочерним элементам Content
        foreach (Transform child in content) {
            // Проверяем, является ли дочерний элемент Image
            if (child.TryGetComponent(out Image image)) {
                // Генерируем случайный цвет
                Color randomColor = new(Random.value, Random.value, Random.value);
                // Применяем случайный цвет к Image
                image.color = randomColor;
            }
        }
    }

}
