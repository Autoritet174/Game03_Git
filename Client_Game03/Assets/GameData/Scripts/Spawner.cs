using UnityEngine;

public class Spawner : MonoBehaviour {

    [Header("Настройки спавна")]
    public GameObject units;
    // Объект, в который будем добавлять спрайт
    //public Sprite spriteToSpawn;   // Спрайт для создания
    //public Vector2 spawnPosition;   // Локальные координаты относительно parentObject

    public Vector2 spriteSize = new(1, 1);
    public int sortingOrder = 0;

    [Header("Настройки цвета")]
    public bool useRandomColor = true;
    public Color fixedColor = Color.white; // Используется, если useRandomColor = false

    public int pixelsPerUnit = 100;
    //[Header("Настройки спрайта")]
    //public Color spriteColor = Color.white;
    //public string spriteName = "NewSprite";
    //public int sortingOrder = 0;
    //public Material spriteMaterial; // Опционально

    private Vector3 _localScale = new(1, 16f / 9f, 1);

    private void Start() {
        Spawn();
    }

    private void Spawn() { 
        // Создаем текстуру 4x4 пикселя (минимальный размер для FullRect)
        Texture2D texture = new(4, 4, TextureFormat.RGBA32, false) {
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Point
        }; 
        // Заполняем текстуру белым цветом
        Color32[] pixels = new Color32[4 * 4];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }
        texture.SetPixels32(pixels);
        texture.Apply();

        // Создаем спрайт с FullRect
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f), // Pivot в центре
            pixelsPerUnit,
            0,
            SpriteMeshType.FullRect
        );

        for (int teamIndex = 1; teamIndex <= 2; teamIndex ++)
        for (int gameObjectIndex = 0; gameObjectIndex < 11; gameObjectIndex++) {

            // Создаем новый игровой объект
            GameObject gameObject = new($"UnitTeam{teamIndex}_{gameObjectIndex}");

            // Добавляем компонент SpriteRenderer
            SpriteRenderer renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.drawMode = SpriteDrawMode.Tiled;
            renderer.tileMode = SpriteTileMode.Continuous;
            renderer.size = spriteSize;

            // Устанавливаем цвет
            renderer.color = useRandomColor ? GetRandomColor() : fixedColor;

            //renderer.color = spriteColor;
            //renderer.sortingOrder = sortingOrder;

            // Назначаем материал, если указан
            //if (spriteMaterial != null) {
            //    renderer.material = spriteMaterial;
            //}

            float x = gameObjectIndex % 2 == 0 ? gameObjectIndex / 2 * 1.5f : -(gameObjectIndex + 1) / 2 * 1.5f;
            float y = teamIndex==1? - 2 : 2;
            Vector2 spawnPosition = new(x, y);
            // Устанавливаем родителя и позицию
            if (units != null) {
                gameObject.transform.SetParent(units.transform);
                gameObject.transform.localPosition = spawnPosition;
                gameObject.transform.localScale = _localScale;
            }
            else {
                gameObject.transform.position = spawnPosition;
            }

            // Настройки рендеринга
            renderer.sortingOrder = sortingOrder;
            renderer.maskInteraction = SpriteMaskInteraction.None;
        }
    }

    // Update is called once per frame
    private void Update() {

    }
    private Color GetRandomColor() {
        return new Color(
            Random.Range(0f, 1f), // R
            Random.Range(0f, 1f), // G
            Random.Range(0f, 1f), // B
            1f                    // A (непрозрачный)
        );
    }
}
