using General.GameEntities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageHeroHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private HeroBaseEntity hero;
    private Sprite normalSprite;
    private Sprite hoverSprite;
    private System.Action<HeroBaseEntity> clickCallback;
    private Image imageComponent;

    public void Initialize(HeroBaseEntity hero, Sprite normal, Sprite hover, System.Action<HeroBaseEntity> callback, Image imageComponent)
    {
        this.hero = hero;
        normalSprite = normal;
        hoverSprite = hover;
        clickCallback = callback;
        this.imageComponent = imageComponent;

        //imageComponent = GetComponent<Image>();
        if (imageComponent != null && normalSprite != null)
        {
            imageComponent.sprite = normalSprite;
        }

        // Включаем Raycast Target для обработки событий
        if (imageComponent != null)
        {
            imageComponent.raycastTarget = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSprite != null && imageComponent != null)
            imageComponent.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (normalSprite != null && imageComponent != null)
            imageComponent.sprite = normalSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickCallback?.Invoke(hero);
    }

    // Обновление параметров
    public void UpdateParameter(HeroBaseEntity hero)
    {
        this.hero = hero;
    }

    public void UpdateSprites(Sprite newNormal, Sprite newHover)
    {
        normalSprite = newNormal;
        hoverSprite = newHover;

        if (imageComponent != null && normalSprite != null)
        {
            imageComponent.sprite = normalSprite;
        }
    }
}