using UnityEngine;

public class HypercubeInit : MonoBehaviour
{
    [Header("Настройки граней (Плоскостей)")]
    public Texture2D facetTexture;
    public Color faceTint = Color.white;
    [Range(0f, 1f)] public float transparency = 0.4f;

    [Tooltip("Цвета для каждой из 24 граней. Если массив пустой или элементов меньше 24, применится faceTint.")]
    public Color[] faceColors = new Color[24]; // Массив для индивидуальных цветов

    [Header("Настройки ребер (Линий)")]
    public Material lineMaterial;
    public float lineWidth = 0.04f;
    public Color lineColor = Color.white;

    [Header("Активация осей вращения")]
    public bool rotateXY = true;
    public bool rotateXZ = true;
    public bool rotateXW = true;
    public bool rotateYZ = true;
    public bool rotateYW = true;
    public bool rotateZW = true;

    [Header("Скорости вращения (если ось активна)")]
    public float speedXY = 0.1f;
    public float speedXZ = 0.1f;
    public float speedXW = 0.5f;
    public float speedYZ = 0f;
    public float speedYW = 0.5f;
    public float speedZW = 0f;

    private float? xy => rotateXY ? speedXY : null;
    private float? xz => rotateXZ ? speedXZ : null;
    private float? xw => rotateXW ? speedXW : null;
    private float? yz => rotateYZ ? speedYZ : null;
    private float? yw => rotateYW ? speedYW : null;
    private float? zw => rotateZW ? speedZW : null;

    [Header("Настройки 4D -> 3D проекции")]
    public float wDistance = 3f;
    public float objectScale = 7f;

    [Header("Настройки 3D -> 2D перспективы")]
    public float camera3DDepth = 4f;
    public bool use2DPerspective = true;

    private Vector4[] points;
    private LineRenderer[] lines;
    private int[,] edges;
    private int[,] faces;
    private Mesh[] meshes;
    private MeshRenderer[] meshRenderers; // Сохраняем ссылки на рендереры для смены цвета на лету

    private float angleXY, angleXZ, angleXW, angleYZ, angleYW, angleZW;
    private bool isInitialized = false;

    private void Start()
    {
        points = new Vector4[16];
        lines = new LineRenderer[32];
        edges = new int[32, 2];
        faces = new int[24, 4];
        meshes = new Mesh[24];
        meshRenderers = new MeshRenderer[24];

        // Инициализация массива цветов по умолчанию, если пользователь его не настроил
        if (faceColors == null || faceColors.Length < 24)
        {
            System.Array.Resize(ref faceColors, 24);
        }
        for (int i = 0; i < 24; i++)
        {
            // Переводим HSV в стандартный Unity RGB Color.
            // Насыщенность (Saturation) и Яркость (Value) выставляем на максимум (1.0),
            // чтобы цвета получились сочными и четкими.
            //faceColors[i] = Color.HSVToRGB(i / 24f, 1f, 1f); // Распределяем тон (Hue) равномерно от 0.0 до 1.0
            faceColors[i] = new Color(64 / 255f, 217 / 255f, 71 / 255f);//41D947
        }
        // ---------------------------------------------------------------------

        // --- 1. Вершины ---
        for (int i = 0; i < 16; i++)
        {
            float x = (i & 1) == 0 ? -1 : 1;
            float y = (i & 2) == 0 ? -1 : 1;
            float z = (i & 4) == 0 ? -1 : 1;
            float w = (i & 8) == 0 ? -1 : 1;
            points[i] = new Vector4(x, y, z, w);
        }

        // --- 2. Генерация объектов Ребер ---
        int lineIndex = 0;
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int bit = 1 << j;
                if ((i & bit) == 0)
                {
                    edges[lineIndex, 0] = i;
                    edges[lineIndex, 1] = i | bit;

                    var lineObj = new GameObject("Line_" + lineIndex);
                    lineObj.transform.parent = transform;
                    lineObj.transform.localPosition = Vector3.zero;

                    LineRenderer lr = lineObj.AddComponent<LineRenderer>();
                    lines[lineIndex] = lr;

                    lineIndex++;
                }
            }
        }

        isInitialized = true;

        // Применяем настройки ребер
        UpdateLineSettings();

        // Базовый шаблон материала граней
        var baseFaceMat = new Material(Shader.Find("Sprites/Default"));
        if (facetTexture != null)
        {
            baseFaceMat.mainTexture = facetTexture;
        }

        // --- 3. Грани (Плоскости) ---
        int faceIndex = 0;
        for (int i = 0; i < 16; i++)
        {
            for (int d1 = 0; d1 < 4; d1++)
            {
                for (int d2 = d1 + 1; d2 < 4; d2++)
                {
                    int b1 = 1 << d1;
                    int b2 = 1 << d2;

                    if ((i & b1) == 0 && (i & b2) == 0)
                    {
                        faces[faceIndex, 0] = i;
                        faces[faceIndex, 1] = i | b1;
                        faces[faceIndex, 2] = i | b1 | b2;
                        faces[faceIndex, 3] = i | b2;

                        var faceObj = new GameObject("Face_" + faceIndex);
                        faceObj.transform.parent = transform;
                        faceObj.transform.localPosition = Vector3.zero;

                        MeshFilter mf = faceObj.AddComponent<MeshFilter>();
                        MeshRenderer mr = faceObj.AddComponent<MeshRenderer>();

                        // Создаем уникальный материал для этой грани
                        mr.material = new Material(baseFaceMat);
                        meshRenderers[faceIndex] = mr;

                        var mesh = new Mesh
                        {
                            name = "FaceMesh_" + faceIndex
                        };
                        mf.mesh = mesh;
                        meshes[faceIndex] = mesh;

                        faceIndex++;
                    }
                }
            }
        }

        // Применяем цвета граней, заданные в инспекторе
        UpdateFaceColors();
    }

    /// <summary>
    /// Обновление параметров ребер
    /// </summary>
    public void UpdateLineSettings()
    {
        if (!isInitialized || lines == null)
        {
            return;
        }

        if (lineMaterial == null)
        {
            lineMaterial = new Material(Shader.Find("Sprites/Default"));
        }

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] != null)
            {
                LineRenderer lr = lines[i];
                lr.useWorldSpace = false;
                lr.material = lineMaterial;
                lr.startWidth = lineWidth;
                lr.endWidth = lineWidth;
                lr.startColor = lineColor;
                lr.endColor = lineColor;
                lr.sortingOrder = 5;
                lr.alignment = LineAlignment.View;
                lr.numCapVertices = 4;
            }
        }
    }

    /// <summary>
    /// Динамическое обновление цветов всех 24 плоскостей на лету
    /// </summary>
    public void UpdateFaceColors()
    {
        if (!isInitialized || meshRenderers == null || faceColors == null)
        {
            return;
        }

        for (int i = 0; i < 24; i++)
        {
            if (meshRenderers[i] != null)
            {
                // Если в инспекторе урезали массив, берем дефолтный faceTint
                Color chosenColor = (i < faceColors.Length) ? faceColors[i] : faceTint;

                // Накладываем прозрачность из общего ползунка transparency
                chosenColor.a = transparency;

                meshRenderers[i].material.color = chosenColor;
            }
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateLineSettings();
            UpdateFaceColors(); // Перерисовываем грани при изменении цвета в инспекторе
        }
    }

    private void Update()
    {
        // Константа полного оборота в радианах (360 градусов)
        float twoPi = Mathf.PI * 2f;

        // Накапливаем угол и зацикливаем его в пределах от 0 до 2*Pi
        angleXY = xy.HasValue ? (angleXY + (xy.Value * Time.deltaTime)) % twoPi : 0f;
        angleXZ = xz.HasValue ? (angleXZ + (xz.Value * Time.deltaTime)) % twoPi : 0f;
        angleXW = xw.HasValue ? (angleXW + (xw.Value * Time.deltaTime)) % twoPi : 0f;
        angleYZ = yz.HasValue ? (angleYZ + (yz.Value * Time.deltaTime)) % twoPi : 0f;
        angleYW = yw.HasValue ? (angleYW + (yw.Value * Time.deltaTime)) % twoPi : 0f;
        angleZW = zw.HasValue ? (angleZW + (zw.Value * Time.deltaTime)) % twoPi : 0f;

        // Если скорость была отрицательной, остаток может быть меньше нуля — корректируем:
        if (angleXY < 0f)
        {
            angleXY += twoPi;
        }

        if (angleXZ < 0f)
        {
            angleXZ += twoPi;
        }

        if (angleXW < 0f)
        {
            angleXW += twoPi;
        }

        if (angleYZ < 0f)
        {
            angleYZ += twoPi;
        }

        if (angleYW < 0f)
        {
            angleYW += twoPi;
        }

        if (angleZW < 0f)
        {
            angleZW += twoPi;
        }

        var projected2DPoints = new Vector3[16];

        for (int i = 0; i < 16; i++)
        {
            Vector4 v = points[i];

            if (angleXY != 0f)
            {
                v = RotateXY(v, angleXY);
            }

            if (angleXZ != 0f)
            {
                v = RotateXZ(v, angleXZ);
            }

            if (angleXW != 0f)
            {
                v = RotateXW(v, angleXW);
            }

            if (angleYZ != 0f)
            {
                v = RotateYZ(v, angleYZ);
            }

            if (angleYW != 0f)
            {
                v = RotateYW(v, angleYW);
            }

            if (angleZW != 0f)
            {
                v = RotateZW(v, angleZW);
            }

            float distance4D = wDistance - v.w;
            if (distance4D < 0.2f)
            {
                distance4D = 0.2f;
            }

            float wFactor = 1f / distance4D;

            float x3D = v.x * wFactor;
            float y3D = v.y * wFactor;
            float z3D = v.z * wFactor;

            float x2D = x3D;
            float y2D = y3D;

            if (use2DPerspective)
            {
                float distance3D = camera3DDepth - z3D;
                if (distance3D < 0.2f)
                {
                    distance3D = 0.2f;
                }

                float zFactor = 1f / distance3D;

                x2D = x3D * zFactor * camera3DDepth;
                y2D = y3D * zFactor * camera3DDepth;
            }

            projected2DPoints[i] = new Vector3(x2D, y2D, 0f) * objectScale;
        }

        for (int i = 0; i < 32; i++)
        {
            lines[i].SetPosition(0, projected2DPoints[edges[i, 0]]);
            lines[i].SetPosition(1, projected2DPoints[edges[i, 1]]);
        }

        for (int i = 0; i < 24; i++)
        {
            Mesh mesh = meshes[i];

            mesh.vertices = new Vector3[]
            {
                projected2DPoints[faces[i, 0]],
                projected2DPoints[faces[i, 1]],
                projected2DPoints[faces[i, 2]],
                projected2DPoints[faces[i, 3]]
            };

            mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3, 2, 1, 0, 3, 2, 0 };

            mesh.uv = new Vector2[]
            {
                new(0, 0), new(1, 0), new(1, 1), new(0, 1)
            };

            mesh.RecalculateBounds();
        }
    }

    private Vector4 RotateXY(Vector4 v, float rad)
    { float s = Mathf.Sin(rad), c = Mathf.Cos(rad); return new Vector4((v.x * c) - (v.y * s), (v.x * s) + (v.y * c), v.z, v.w); }
    private Vector4 RotateXZ(Vector4 v, float rad)
    { float s = Mathf.Sin(rad), c = Mathf.Cos(rad); return new Vector4((v.x * c) - (v.z * s), v.y, (v.x * s) + (v.z * c), v.w); }
    private Vector4 RotateXW(Vector4 v, float rad)
    { float s = Mathf.Sin(rad), c = Mathf.Cos(rad); return new Vector4((v.x * c) - (v.w * s), v.y, v.z, (v.x * s) + (v.w * c)); }
    private Vector4 RotateYZ(Vector4 v, float rad)
    { float s = Mathf.Sin(rad), c = Mathf.Cos(rad); return new Vector4(v.x, (v.y * c) - (v.z * s), (v.y * s) + (v.z * c), v.w); }
    private Vector4 RotateYW(Vector4 v, float rad)
    { float s = Mathf.Sin(rad), c = Mathf.Cos(rad); return new Vector4(v.x, (v.y * c) - (v.w * s), v.z, (v.y * s) + (v.w * c)); }
    private Vector4 RotateZW(Vector4 v, float rad)
    { float s = Mathf.Sin(rad), c = Mathf.Cos(rad); return new Vector4(v.x, v.y, (v.z * c) - (v.w * s), (v.z * s) + (v.w * c)); }
}
