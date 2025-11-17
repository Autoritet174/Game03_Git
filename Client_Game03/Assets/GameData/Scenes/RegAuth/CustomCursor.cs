using UnityEngine;

/// <summary>
/// Управляет отображением пользовательского курсора без задержки.
/// </summary>
public class CustomCursor : MonoBehaviour
{
    /// <summary>Текстура курсора (RGBA, без mipmaps).</summary>
    [SerializeField]
    private Texture2D cursorTexture;

    private void Start()
    {
        if (cursorTexture == null)
        {
            throw new System.Exception("Cursor texture is null.");
        }

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}
