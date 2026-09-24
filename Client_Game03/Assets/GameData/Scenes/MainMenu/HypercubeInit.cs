using UnityEngine;

/// <summary>Создаёт и вращает проекцию гиперкуба с восстановлением геометрии после перезагрузки сборок.</summary>
public class HypercubeInit : MonoBehaviour
{
    /// <summary>Текстура граней гиперкуба.</summary>
    [Header("Настройки граней (Плоскостей)")]
    public Texture2D facetTexture;
    /// <summary>Цвет грани при отсутствии индивидуального цвета.</summary>
    public Color faceTint = Color.white;
    /// <summary>Прозрачность граней.</summary>
    [Range(0f, 1f)]
    public float transparency = 0.4f;

    /// <summary>Индивидуальные цвета 24 граней.</summary>
    [Tooltip("Цвета для каждой из 24 граней. Если массив пустой или элементов меньше 24, применится faceTint.")]
    public Color[] faceColors = new Color[24]; // Массив для индивидуальных цветов

    /// <summary>Материал рёбер.</summary>
    [Header("Настройки ребер (Линий)")]
    public Material lineMaterial;
    /// <summary>Толщина рёбер.</summary>
    public float lineWidth = 0.04f;
    /// <summary>Цвет рёбер.</summary>
    public Color lineColor = Color.white;

    /// <summary>Разрешает вращение в плоскости XY.</summary>
    [Header("Активация осей вращения")]
    public bool rotateXY = true;
    /// <summary>Разрешает вращение в плоскости XZ.</summary>
    public bool rotateXZ = true;
    /// <summary>Разрешает вращение в плоскости XW.</summary>
    public bool rotateXW = true;
    /// <summary>Разрешает вращение в плоскости YZ.</summary>
    public bool rotateYZ = true;
    /// <summary>Разрешает вращение в плоскости YW.</summary>
    public bool rotateYW = true;
    /// <summary>Разрешает вращение в плоскости ZW.</summary>
    public bool rotateZW = true;

    /// <summary>Скорость вращения в плоскости XY в радианах в секунду.</summary>
    [Header("Скорости вращения (если ось активна)")]
    public float speedXY = 0.1f;
    /// <summary>Скорость вращения в плоскости XZ в радианах в секунду.</summary>
    public float speedXZ = 0.1f;
    /// <summary>Скорость вращения в плоскости XW в радианах в секунду.</summary>
    public float speedXW = 0.5f;
    /// <summary>Скорость вращения в плоскости YZ в радианах в секунду.</summary>
    public float speedYZ = 0f;
    /// <summary>Скорость вращения в плоскости YW в радианах в секунду.</summary>
    public float speedYW = 0.5f;
    /// <summary>Скорость вращения в плоскости ZW в радианах в секунду.</summary>
    public float speedZW = 0f;

    /// <summary>Активная скорость вращения в плоскости XY либо отсутствие вращения.</summary>
    private float? xy => rotateXY ? speedXY : null;
    /// <summary>Активная скорость вращения в плоскости XZ либо отсутствие вращения.</summary>
    private float? xz => rotateXZ ? speedXZ : null;
    /// <summary>Активная скорость вращения в плоскости XW либо отсутствие вращения.</summary>
    private float? xw => rotateXW ? speedXW : null;
    /// <summary>Активная скорость вращения в плоскости YZ либо отсутствие вращения.</summary>
    private float? yz => rotateYZ ? speedYZ : null;
    /// <summary>Активная скорость вращения в плоскости YW либо отсутствие вращения.</summary>
    private float? yw => rotateYW ? speedYW : null;
    /// <summary>Активная скорость вращения в плоскости ZW либо отсутствие вращения.</summary>
    private float? zw => rotateZW ? speedZW : null;

    /// <summary>Расстояние наблюдателя для проекции из четырёх измерений.</summary>
    [Header("Настройки 4D -> 3D проекции")]
    public float wDistance = 3f;
    /// <summary>Общий масштаб проекции.</summary>
    public float objectScale = 7f;

    /// <summary>Глубина камеры для перспективной проекции.</summary>
    [Header("Настройки 3D -> 2D перспективы")]
    public float camera3DDepth = 4f;
    /// <summary>Признак использования перспективы при проекции на плоскость.</summary>
    public bool use2DPerspective = true;

    /// <summary>Шестнадцать исходных четырёхмерных вершин.</summary>
    private Vector4[] points;
    /// <summary>Компоненты отображения 32 рёбер.</summary>
    private LineRenderer[] lines;
    /// <summary>Индексы концов рёбер; многомерный массив восстанавливается после перезагрузки сборок.</summary>
    private int[,] edges;
    /// <summary>Индексы вершин граней; многомерный массив восстанавливается после перезагрузки сборок.</summary>
    private int[,] faces;
    /// <summary>Меши 24 граней.</summary>
    private Mesh[] meshes;
    /// <summary>Компоненты отображения граней.</summary>
    private MeshRenderer[] meshRenderers; // Сохраняем ссылки на рендереры для смены цвета на лету

    /// <summary>Текущие углы поворота в шести координатных плоскостях.</summary>
    private float angleXY, angleXZ, angleXW, angleYZ, angleYW, angleZW;
    /// <summary>Признак полного завершения подготовки геометрии.</summary>
    private bool isInitialized = false;

    /// <summary>Восстанавливает геометрию при запуске и после перезагрузки сборок в Play Mode.</summary>
    private void OnEnable()
    {
        if (Application.isPlaying)
            InitializeGeometry();
    }

    /// <summary>Восстанавливает несериализуемые таблицы связей, переиспользуя существующие объекты, материалы и меши.</summary>
    private void InitializeGeometry()
    {
        bool initializeColors = !isInitialized;
        isInitialized = false;
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
        for (int i = 0; initializeColors && i < 24; i++)
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

                    Transform existingLine = transform.Find("Line_" + lineIndex);
                    GameObject lineObj = existingLine != null ? existingLine.gameObject : new GameObject("Line_" + lineIndex);
                    lineObj.transform.SetParent(transform, false);
                    lineObj.transform.localPosition = Vector3.zero;

                    LineRenderer lr = lineObj.GetComponent<LineRenderer>();
                    if (lr == null)
                        lr = lineObj.AddComponent<LineRenderer>();
                    lines[lineIndex] = lr;

                    lineIndex++;
                }
            }
        }

        Shader faceShader = Shader.Find("Sprites/Default");

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

                        Transform existingFace = transform.Find("Face_" + faceIndex);
                        GameObject faceObj = existingFace != null ? existingFace.gameObject : new GameObject("Face_" + faceIndex);
                        faceObj.transform.SetParent(transform, false);
                        faceObj.transform.localPosition = Vector3.zero;

                        MeshFilter mf = faceObj.GetComponent<MeshFilter>();
                        if (mf == null)
                            mf = faceObj.AddComponent<MeshFilter>();
                        MeshRenderer mr = faceObj.GetComponent<MeshRenderer>();
                        if (mr == null)
                            mr = faceObj.AddComponent<MeshRenderer>();

                        if (mr.sharedMaterial == null)
                            mr.sharedMaterial = new Material(faceShader);
                        mr.sharedMaterial.mainTexture = facetTexture;
                        meshRenderers[faceIndex] = mr;

                        if (mf.sharedMesh == null)
                            mf.sharedMesh = new Mesh { name = "FaceMesh_" + faceIndex };
                        meshes[faceIndex] = mf.sharedMesh;

                        faceIndex++;
                    }
                }
            }
        }

        isInitialized = true;
        UpdateLineSettings();
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
                lr.sharedMaterial = lineMaterial;
                lr.positionCount = 2;
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

                meshRenderers[i].sharedMaterial.color = chosenColor;
            }
        }
    }

    /// <summary>Применяет изменённые в инспекторе настройки к готовой геометрии.</summary>
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateLineSettings();
            UpdateFaceColors(); // Перерисовываем грани при изменении цвета в инспекторе
        }
    }

    /// <summary>Вращает вершины и обновляет проекции готовых рёбер и граней.</summary>
    private void Update()
    {
        if (!isInitialized || points == null || lines == null || edges == null || faces == null || meshes == null)
            return;

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
            LineRenderer line = lines[i];
            if (line == null)
                continue;
            line.SetPosition(0, projected2DPoints[edges[i, 0]]);
            line.SetPosition(1, projected2DPoints[edges[i, 1]]);
        }

        for (int i = 0; i < 24; i++)
        {
            Mesh mesh = meshes[i];
            if (mesh == null)
                continue;

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

    /// <summary>Возвращает вершину после поворота в плоскости XY.</summary>
    private Vector4 RotateXY(Vector4 v, float rad)
    { float s = Mathf.Sin(rad), c = Mathf.Cos(rad); return new Vector4((v.x * c) - (v.y * s), (v.x * s) + (v.y * c), v.z, v.w); }
    /// <summary>Возвращает вершину после поворота в плоскости XZ.</summary>
    private Vector4 RotateXZ(Vector4 v, float rad)
    { float s = Mathf.Sin(rad), c = Mathf.Cos(rad); return new Vector4((v.x * c) - (v.z * s), v.y, (v.x * s) + (v.z * c), v.w); }
    /// <summary>Возвращает вершину после поворота в плоскости XW.</summary>
    private Vector4 RotateXW(Vector4 v, float rad)
    { float s = Mathf.Sin(rad), c = Mathf.Cos(rad); return new Vector4((v.x * c) - (v.w * s), v.y, v.z, (v.x * s) + (v.w * c)); }
    /// <summary>Возвращает вершину после поворота в плоскости YZ.</summary>
    private Vector4 RotateYZ(Vector4 v, float rad)
    { float s = Mathf.Sin(rad), c = Mathf.Cos(rad); return new Vector4(v.x, (v.y * c) - (v.z * s), (v.y * s) + (v.z * c), v.w); }
    /// <summary>Возвращает вершину после поворота в плоскости YW.</summary>
    private Vector4 RotateYW(Vector4 v, float rad)
    { float s = Mathf.Sin(rad), c = Mathf.Cos(rad); return new Vector4(v.x, (v.y * c) - (v.w * s), v.z, (v.y * s) + (v.w * c)); }
    /// <summary>Возвращает вершину после поворота в плоскости ZW.</summary>
    private Vector4 RotateZW(Vector4 v, float rad)
    { float s = Mathf.Sin(rad), c = Mathf.Cos(rad); return new Vector4(v.x, v.y, (v.z * c) - (v.w * s), (v.z * s) + (v.w * c)); }
}
