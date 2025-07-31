using General;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Initializator1 : MonoBehaviour
{
    //// Ссылка на панель, в которую будут добавляться элементы
    //public Transform contentPanel;
    //// Префаб квадрата с текстурой
    //public GameObject squarePrefab;
    //public GameObject content;

    //// Текстуры для квадратиков
    //public Sprite[] textures;

    //List<GameObject> heroes = new();
    //private LayoutGroup layoutGroup; // Ссылка на LayoutGroup для обновления

    //void Start() {
    //    Debug.Log(123);
    //    // Получаем ссылку на LayoutGroup, чтобы обновлять расположение элементов
    //    layoutGroup = contentPanel.GetComponent<LayoutGroup>();
    //    // Пример добавления нескольких объектов
    //    for (int i = 0; i < 20; i++) // например 10 объектов
    //    {
    //        AddSquareToPanel(i);
    //    }
    //    // Принудительное обновление LayoutGroup после добавления элементов
    //    Canvas.ForceUpdateCanvases();
    //    layoutGroup.SetLayoutVertical();  // Обновление вертикального расположения
    //}

    //// Метод для добавления квадратика в панель
    //void AddSquareToPanel(int index)
    //{
    //    Debug.Log(234);
    //    GameObject square = Instantiate(squarePrefab);
    //    heroes.Add(square);
    //    square.transform.SetParent(contentPanel, false); // Устанавливаем родительский объект (Content)

    //    // Устанавливаем текстуру для квадратика
    //    Image image = square.GetComponent<Image>();
    //    if (textures != null && textures.Length > index) {
    //        image.sprite = textures[index];
    //    }
    //    Debug.Log(1);
    //    square.transform.localScale = Vector3.one*100;
    //    square.transform.localPosition = new Vector3(110, -110 * index + 50, 0);
    //}
}
