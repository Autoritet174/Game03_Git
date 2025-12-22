using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
using General.DTO;
using General.DTO.Entities.GameData;

public class ImageHeroHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private DtoBaseHero hero;
    private Sprite normalSprite;
    private Sprite hoverSprite;
    private System.Func<DtoBaseHero, Task> clickCallback;
    private Image imageComponent;

    public void Initialize(DtoBaseHero hero, Sprite normal, Sprite hover, System.Func<DtoBaseHero, Task> callback, Image imageComponent)
    {
        this.hero = hero;
        normalSprite = normal;
        hoverSprite = hover;
        clickCallback = callback;
        this.imageComponent = imageComponent;

        if (imageComponent != null && normalSprite != null)
        {
            imageComponent.sprite = normalSprite;
        }

        if (imageComponent != null)
        {
            imageComponent.raycastTarget = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSprite != null && imageComponent != null)
        {
            imageComponent.sprite = hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (normalSprite != null && imageComponent != null)
        {
            imageComponent.sprite = normalSprite;
        }
    }

    /// <summary>
    /// Асинхронный обработчик клика
    /// </summary>
    /// <param name="eventData"></param>
    public async void OnPointerClick(PointerEventData eventData)
    {
        if (clickCallback != null)
        {
            // Вызываем асинхронный callback и ждем результат
            await clickCallback(hero);
            // Можно обработать результат если нужно
        }
    }


    private async System.Threading.Tasks.Task HandleClickAsync()
    {
        try
        {
            await clickCallback(hero);
            // Обработка результата
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in click handler: {ex.Message}");
        }
    }

    public void UpdateParameter(DtoBaseHero hero)
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
