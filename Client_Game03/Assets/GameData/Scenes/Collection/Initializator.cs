using Assets.GameData.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using L = General.LocalizationKeys;

namespace Assets.GameData.Scenes.Collection
{
    public class Initializator : MonoBehaviour
    {
        public Initializator() {
            viewer = new(this);
            panelTop = new(this);
        }
        private readonly PanelCollection viewer;
        private readonly PanelTop panelTop;

        public static readonly IEnumerable<string> Slots1by1 = new[] { "Head", "Armor", "Hands", "Feet", "Waist", "Neck", "Shield" };

        public static readonly Vector3 Vector3Selected = new(1.15f, 1.15f, 1);

        //public bool Initialized { get; private set; }
        private bool _initialized = false;
        public bool ScrollbarVertical_Active { get; private set; } = false;
        private float _width, _height;


        
        private Image _Background_Image;
        private float _ImageBackgroundCoef = 1;

       
        private RectTransform _ButtonCloseSelectedHero_RectTransform;
        private RectTransform _ButtonCloseSelectedEquipment_RectTransform;
        
        public bool PanelSelectedHeroIsActive { get; set; } = false;
        public bool PanelSelectedEquipmentIsActive { get; set; } = false;
        
   
        private RectTransform _PanelSelectedHeroBottomTab1_RectTransform;

        public GameObject PanelSelectedEquipment_GameObject { get; private set; }
        private RectTransform _PanelSelectedEquipment_RectTransform;
        private RectTransform _PanelSelectedEquipmentTop_RectTransform;
        private RectTransform _PanelSelectedEquipmentBottom_RectTransform;
        private RectTransform _PanelSelectedEquipmentBottomTabButton1_RectTransform;
        private RectTransform _PanelSelectedEquipmentBottomTabButton2_RectTransform;
        private TextMeshProUGUI _PanelSelectedEquipmentBottomTabButton1_TextMeshProUGUI;
        private TextMeshProUGUI _PanelSelectedEquipmentBottomTabButton2_TextMeshProUGUI;
        private RectTransform _PanelSelectedEquipmentBottomTab1_RectTransform;


        private RectTransform _ScrollViewCollection_RectTransform;
        public RectTransform ScrollbarVertical_RectTransform { get; private set; }
        private GameObject ScrollbarVertical_GameObject;

       
        public TextMeshProUGUI SelectedEquipmentTop_TextMeshProUGUI { get; private set; }
        private RectTransform _SelectedEquipmentImageContainer_RectTransform;


        public Image SelectedHero_Image { get; private set; }
        public Image SelectedHeroRarity_Image { get; private set; }
        public Guid SelectedHeroId { get; set; }

        public Image SelectedEquipment_Image { get; private set; }
        public Image SelectedEquipmentRarity_Image { get; private set; }
        public Guid SelectedEquipmentId { get; set; }


        public RectTransform ButtonTakeOnOff_RectTransform { get; private set; }
        public TextMeshProUGUI ButtonTakeOnOff_TextMeshProUGUI { get; private set; }
        public RectTransform ButtonSell_RectTransform { get; private set; }
        public TextMeshProUGUI ButtonSell_TextMeshProUGUI { get; private set; }



        private Transform _CollectionContent_Transform;




     
       

        //private readonly int slotIndex = 0;



        private async void Start()
        {
          

            // Изображение заднего фона
            _Background_Image = GameObjectFinder.FindByName<Image>("Image_Background (id=688x18dt)");
            if (_Background_Image != null && _Background_Image.sprite != null)
            {
                Texture2D texture = _Background_Image.sprite.texture;
                _ImageBackgroundCoef = texture.width / (float)texture.height;
            }

            
            PanelSelectedHero_GameObject = _PanelSelectedHero_RectTransform.gameObject;
            PanelSelectedHeroSetActive(false, false);
            _PanelSelectedHero_RectTransform.anchoredPosition = Vector2.zero;
            PanelSelectedHeroIsActive = PanelSelectedHero_GameObject.activeInHierarchy;

            _PanelSelectedEquipment_RectTransform = ;
            PanelSelectedEquipment_GameObject = _PanelSelectedEquipment_RectTransform.gameObject;
            PanelSelectedEquipmentSetActive(false, false);
            _PanelSelectedEquipment_RectTransform.anchoredPosition = Vector2.zero;
            PanelSelectedEquipmentIsActive = PanelSelectedEquipment_GameObject.activeInHierarchy;

            ButtonTakeOnOff_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTakeOnOff (id=fllqlepl)");
            ButtonTakeOnOff_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTakeOnOffText (id=xfqoucqj)");
            ButtonSell_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonSell (id=sp1vha3z)");
            ButtonSell_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonSellText (id=b68za6o5)");
            ButtonSell_TextMeshProUGUI.text = Game03Client.LocalizationManager.GetValue(L.UI.Button.Sell);

            SelectedHero_Image = GameObjectFinder.FindByName<Image>("ImageHeroFull (id=m5kn2f6p)");
            SelectedHeroRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity (id=xami3s9q)");

            SelectedEquipment_Image = GameObjectFinder.FindByName<Image>("ImageEquipmentFull (id=gu7wtz83)");
            SelectedEquipmentRarity_Image = GameObjectFinder.FindByName<Image>("ImageRarity (id=qje8dq78)");

            
            _SelectedEquipmentImageContainer_RectTransform = GameObjectFinder.FindByName<RectTransform>("Image_Container (id=bqxjhczr)");

           


           
           


            _ButtonCloseSelectedEquipment_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonClose (id=va8d3lsz)");
            _ButtonCloseSelectedEquipment_RectTransform.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                PanelSelectedEquipmentSetActive(false, false);
                foreach (GroupDivider a in _GroupDividers)
                {
                    bool founded = false;
                    foreach (GroupDivider.DataCollectionElement b in a.ListDataCollectionElement)
                    {
                        if (b.Selected)
                        {
                            b.rectTransform.localScale = Vector3.one;
                            founded = true;
                            break;
                        }
                    }
                    if (founded)
                    {
                        break;
                    }
                }
                OnResizeWindow();
            });

            

            _PanelSelectedEquipmentTop_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipmentTop (id=dp54agcp)");
            _PanelSelectedEquipmentBottom_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipmentBottom (id=bj3zvapm)");

           

            _PanelSelectedEquipmentBottomTabButton1_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab1 (id=n94o21t8)");
            _PanelSelectedEquipmentBottomTabButton1_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab1Text (id=yjb1gqbc)");
            _PanelSelectedEquipmentBottomTabButton1_TextMeshProUGUI.SetText(Game03Client.LocalizationManager.GetValue(L.UI.Button.Item));

            _PanelSelectedHeroBottomTabButton2_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab2 (id=kzury0kd)");
            _PanelSelectedHeroBottomTabButton2_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab2Text (id=6bjw6hi4)");
            _PanelSelectedHeroBottomTabButton2_TextMeshProUGUI.SetText("{Tab2}");

            _PanelSelectedEquipmentBottomTabButton2_RectTransform = GameObjectFinder.FindByName<RectTransform>("ButtonTab2 (id=c1xjs5dr)");
            _PanelSelectedEquipmentBottomTabButton2_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("ButtonTab2Text (id=pn28dhfr)");
            _PanelSelectedEquipmentBottomTabButton2_TextMeshProUGUI.SetText("{Tab2}");

            
            

            _PanelSelectedEquipmentBottomTab1_RectTransform = GameObjectFinder.FindByName<RectTransform>("PanelSelectedEquipmentBottomTab1 (id=9nwzj7p8)");
            SelectedEquipmentTop_TextMeshProUGUI = GameObjectFinder.FindByName<TextMeshProUGUI>("Label_SelectedEquipment (id=004gk90y)");

            // Scroll View для коллекции героев
            _ScrollViewCollection_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollViewCollection (id=ph1oh7dk)");
            ScrollbarVertical_RectTransform = GameObjectFinder.FindByName<RectTransform>("ScrollbarVertical (id=ti32ix3l)");
            ScrollbarVertical_GameObject = ScrollbarVertical_RectTransform.gameObject;

            // Коллекция контент
            _CollectionContent_Transform = GameObjectFinder.FindByName("Content (id=ddmjr9vy)").transform;

            


            // Панель навигации по страницам
           
            UpdatePageMax();


            _initialized = true;

            GameMessage.ShowLocale(L.Info.LoadingCollection, false);

            await InstantiateCollectionAsync();
        }

        private void Update()
        {
            if (!_initialized)
            {
                return;
            }

            bool resize = false;
            if (ScrollbarVertical_Active != ScrollbarVertical_GameObject.activeInHierarchy)
            {
                ScrollbarVertical_Active = ScrollbarVertical_GameObject.activeInHierarchy;

                resize = true;
            }
            if (!resize && (!Mathf.Approximately(Screen.height, _height) || !Mathf.Approximately(Screen.width, _width)))
            {
                resize = true;
            }

            if (PanelSelectedHeroIsActive != PanelSelectedHero_GameObject.activeInHierarchy)
            {
                PanelSelectedHeroIsActive = PanelSelectedHero_GameObject.activeInHierarchy;
                resize = true;
            }

            if (PanelSelectedEquipmentIsActive != PanelSelectedEquipment_GameObject.activeInHierarchy)
            {
                PanelSelectedEquipmentIsActive = PanelSelectedEquipment_GameObject.activeInHierarchy;
                resize = true;
            }


            if (resize)
            {
                OnResizeWindow();
            }
        }

       
      

       
        public void UnselectAll()
        {
            for (int i = 0; i < _GroupDividers.Count; i++)
            {
                GroupDivider g = _GroupDividers[i];
                for (int j = 0; j < g.ListDataCollectionElement.Count; j++)
                {
                    GroupDivider.DataCollectionElement el = g.ListDataCollectionElement[j];
                    el.Selected = false;
                    el.rectTransform.localScale = Vector3.one;
                }
            }
        }

       

       

        public void PanelSelectedHeroSetActive(bool active, bool executeOnResizeWindow)
        {
           
        }
        public void PanelSelectedEquipmentSetActive(bool active, bool executeOnResizeWindow)
        {
            PanelSelectedEquipment_GameObject.SetActive(active);
            if (active)
            {
                SelectedEquipmentId = Guid.Empty;
            }
            if (executeOnResizeWindow)
            {
                PanelSelectedEquipmentIsActive = true;
                OnResizeWindow();
            }
        }

        public void OnResizeWindow()
        {
            _height = Screen.height;
            _width = Screen.width;
            float coefHeight = _height / 1080f;
            float fontSize = 25f * coefHeight;

            //buttonHeroesTmp.fontSize = 32f * _lastHeight / 1080;
            //buttonItemsTmp.fontSize = 32f * _lastHeight / 1080;

            // Изображение заднего фона
            float coefScreen = _width / _height;// 10000/1000 = 10 // 1920 / 1080 = 1,7778
            _Background_Image.rectTransform.sizeDelta = coefScreen > _ImageBackgroundCoef ? new Vector2(_width, _width / _ImageBackgroundCoef) : new Vector2(_height * _ImageBackgroundCoef, _height);


            // Верхняя панель
            float topPanelHeightPercent = 0.08f;
            float panelTopHeight = topPanelHeightPercent * _height; // 86.4
            Vector2 vector008PercentOfHeight = new(panelTopHeight, panelTopHeight);
            


            //// Кнопки вкладок
            //float tabButtonWidth = 230.4f * coefHeight;
            //var v = new Vector2(tabButtonWidth, panelTopHeight);
            //_TabButtonHeroes.rectTransform.sizeDelta = v;
            //_TabButtonEquipment.rectTransform.sizeDelta = v;
            //_TabButtonChangingEquipment.rectTransform.sizeDelta = v;

            //_TabButtonEquipment.rectTransform.anchoredPosition = new Vector2(tabButtonWidth, 0);
            //_TabButtonChangingEquipment.rectTransform.anchoredPosition = new Vector2(tabButtonWidth * 2, 0);

            //float f22 = 22f * coefHeight;
            //float f5 = 5f * coefHeight;
            //float f10 = 10f * coefHeight;
            //float f15 = 15f * coefHeight;
            //float f50 = 50f * coefHeight;
            //_TabButtonHeroes.textMeshProUGUI.fontSize = f22;
            //_TabButtonEquipment.textMeshProUGUI.fontSize = f22;
            //_TabButtonChangingEquipment.textMeshProUGUI.fontSize = f22;


            //// Кнопки "Закрыть"
            //_ButtonClose_RectTransform.sizeDelta = vector008PercentOfHeight;


            // Панель выбранного героя
            float panelSelectedHeroWidth = 0;
            if (PanelSelectedHeroIsActive)
            {
                _ButtonCloseSelectedHero_RectTransform.sizeDelta = vector008PercentOfHeight;

                float panelSelectedHeroWidthBase = 535f; // при разрешении 1920x1080
                panelSelectedHeroWidth = panelSelectedHeroWidthBase * coefHeight;

                // Панель выбранного героя
                _PanelSelectedHero_RectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth, 994f * coefHeight);

                

                

             

                // Вкладка 1. Экипировка
                

                // Выбранный герой. Лабел
                SelectedHeroTop_TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(panelSelectedHeroWidth - panelTopHeight, panelTopHeight);
                //SelectedHeroTop_TextMeshProUGUI.rectTransform.anchoredPosition = new Vector2(panelTopHeight, 0);
                SelectedHeroTop_TextMeshProUGUI.fontSize = 30f * coefHeight;

                float f = 460.9983f * coefHeight;
                

            }

            float panelSelectedEquipmentWidth = 0;
            if (PanelSelectedEquipmentIsActive)
            {
                _ButtonCloseSelectedEquipment_RectTransform.sizeDelta = vector008PercentOfHeight;

                float panelSelectedEquipmentWidthBase = 535f; // при разрешении 1920x1080
                panelSelectedEquipmentWidth = panelSelectedEquipmentWidthBase * coefHeight;

                // Панель выбранной экипировки
                _PanelSelectedEquipment_RectTransform.anchoredPosition = panelSelectedHeroWidth > 0 ? new Vector2(-panelSelectedHeroWidth, 0) : Vector2.zero;
                _PanelSelectedEquipment_RectTransform.sizeDelta = new Vector2(panelSelectedEquipmentWidth, 994 * coefHeight);

                // Панель выбранного героя. Верхняя панель где написано название экипировки
                _PanelSelectedEquipmentTop_RectTransform.sizeDelta = new Vector2(panelSelectedEquipmentWidth, panelTopHeight);

                // Панель выбранного героя. Нижняя панель с характеристиками героя
                _PanelSelectedEquipmentBottom_RectTransform.sizeDelta = new Vector2(panelSelectedEquipmentWidth, 908 * coefHeight);

                // Кнопки вкладок
                _PanelSelectedEquipmentBottomTabButton1_RectTransform.sizeDelta = new Vector2(150 * coefHeight, f50);
                _PanelSelectedEquipmentBottomTabButton2_RectTransform.sizeDelta = _PanelSelectedEquipmentBottomTabButton1_RectTransform.sizeDelta;

                _PanelSelectedEquipmentBottomTabButton1_RectTransform.anchoredPosition = new Vector2(f5, -f5);
                _PanelSelectedEquipmentBottomTabButton2_RectTransform.anchoredPosition = new Vector2(160f * coefHeight, -f5);

                _PanelSelectedEquipmentBottomTabButton1_TextMeshProUGUI.fontSize = f15;
                _PanelSelectedEquipmentBottomTabButton2_TextMeshProUGUI.fontSize = f15;

                // Вкладка 1. Экипировка
                _PanelSelectedEquipmentBottomTab1_RectTransform.sizeDelta = new Vector2(panelSelectedEquipmentWidth, 848 * coefHeight);

                // Выбранный герой. Лабел
                SelectedEquipmentTop_TextMeshProUGUI.rectTransform.sizeDelta = new Vector2(panelSelectedEquipmentWidth - panelTopHeight, panelTopHeight);
                //SelectedEquipmentTop_TextMeshProUGUI.rectTransform.anchoredPosition = new Vector2(panelTopHeight, 0);
                SelectedEquipmentTop_TextMeshProUGUI.fontSize = 30f * coefHeight;

                float f = 252.5f * coefHeight;
                _SelectedEquipmentImageContainer_RectTransform.sizeDelta = new Vector2(f, f);

                _SelectedEquipmentImageContainer_RectTransform.anchoredPosition = new Vector2(-f10, f10);

                ButtonTakeOnOff_RectTransform.sizeDelta = new Vector2(121.25f * coefHeight, f50);
                ButtonTakeOnOff_RectTransform.anchoredPosition = new Vector2(f10, f10);

                ButtonSell_RectTransform.sizeDelta = new Vector2(121.25f * coefHeight, f50);
                ButtonSell_RectTransform.anchoredPosition = new Vector2(141.25f * coefHeight, f10);
            }


            // Панель коллекции
            float panelCollection_Width = _width - (panelSelectedHeroWidth + panelSelectedEquipmentWidth);
            PanelCollection_RectTransform.sizeDelta = new Vector2(panelCollection_Width, _height - panelTopHeight);

            // Панель верхних кнопок
            _PanelCollectionTopButtons_RectTransform.sizeDelta = new Vector2(panelCollection_Width, 113f * coefHeight);

            // Внутренние кнопки
            _InternalPanelHeroes.Refresh(coefHeight, vector008PercentOfHeight, 10);
            _InternalPanelEquipments.Refresh(coefHeight, vector008PercentOfHeight, 10);
            _InternalPanelFilter.Refresh(coefHeight, vector008PercentOfHeight, 150);
            _InternalPanelGroup.Refresh(coefHeight, vector008PercentOfHeight, 256);
            _InternalPanelSort.Refresh(coefHeight, vector008PercentOfHeight, 362);

            // Scroll View для коллекции героев
            _ScrollViewCollection_RectTransform.sizeDelta = new Vector2(panelCollection_Width, PanelCollection_RectTransform.sizeDelta.y - _PanelCollectionTopButtons_RectTransform.sizeDelta.y);

            // ScrollbarVertical для коллекции героев
            ScrollbarVertical_RectTransform.sizeDelta = new Vector2(32f * coefHeight, _ScrollViewCollection_RectTransform.sizeDelta.y);

            _CollectionContent_Transform.GetComponent<VerticalLayoutGroup>().spacing = f5;


            // groupDividers
            if (_GroupDividers.Count > 0)
            {
                _GroupDividers.ForEach(a => a.Resize());
            }

            // Панель навигации по страницам
       
        }
    }
}
