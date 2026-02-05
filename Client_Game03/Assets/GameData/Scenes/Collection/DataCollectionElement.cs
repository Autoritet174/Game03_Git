using Game03Client.Collection;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Assets.GameData.Scenes.Collection
{
    public class DataCollectionElement
    {
        public GameObject gameObject;
        public CollectionElement collectionElement;
        public TextMeshProUGUI textMeshPro;
        public bool Selected = false;
        public Image imageRarity;
        public RectTransform rectTransform;
    }
}
