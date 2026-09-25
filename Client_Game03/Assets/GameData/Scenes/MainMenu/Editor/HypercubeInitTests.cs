using NUnit.Framework;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>Проверяет восстановление геометрии гиперкуба после потери несериализуемых таблиц.</summary>
public class HypercubeInitTests
{
    /// <summary>Корневой объект проверяемого гиперкуба.</summary>
    private GameObject root;

    /// <summary>Проверяемый компонент построения и вращения.</summary>
    private HypercubeInit hypercube;

    /// <summary>Создаёт отдельный объект без запуска игровой сцены.</summary>
    [SetUp]
    public void SetUp()
    {
        root = new("Hypercube test");
        hypercube = root.AddComponent<HypercubeInit>();
    }

    /// <summary>Освобождает созданные тестом меши, материалы и объекты.</summary>
    [TearDown]
    public void TearDown()
    {
        foreach (MeshFilter filter in root.GetComponentsInChildren<MeshFilter>())
        {
            Object.DestroyImmediate(filter.sharedMesh);
        }

        foreach (Material material in root.GetComponentsInChildren<Renderer>().Select(renderer => renderer.sharedMaterial).Distinct())
        {
            if (material != null)
            {
                Object.DestroyImmediate(material);
            }
        }

        Object.DestroyImmediate(root);
    }

    /// <summary>Восстанавливает утраченные при reload многомерные массивы без дублирования геометрии и ресурсов.</summary>
    [Test]
    public void ReinitializationRestoresLostTopologyAndReusesGeometry()
    {
        Invoke("InitializeGeometry");
        Invoke("Update");
        LineRenderer[] lines = root.GetComponentsInChildren<LineRenderer>();
        Mesh[] meshes = root.GetComponentsInChildren<MeshFilter>().Select(filter => filter.sharedMesh).ToArray();
        Material[] materials = root.GetComponentsInChildren<Renderer>().Select(renderer => renderer.sharedMaterial).ToArray();
        Assert.That(lines.Length, Is.EqualTo(32));
        Assert.That(meshes.Length, Is.EqualTo(24));
        hypercube.faceColors[0] = Color.magenta;

        typeof(HypercubeInit).GetField("edges", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(hypercube, null);
        typeof(HypercubeInit).GetField("faces", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(hypercube, null);
        Assert.DoesNotThrow(() => Invoke("Update"));
        Invoke("InitializeGeometry");
        Assert.DoesNotThrow(() => Invoke("Update"));

        CollectionAssert.AreEqual(lines, root.GetComponentsInChildren<LineRenderer>());
        CollectionAssert.AreEqual(meshes, root.GetComponentsInChildren<MeshFilter>().Select(filter => filter.sharedMesh));
        CollectionAssert.AreEqual(materials, root.GetComponentsInChildren<Renderer>().Select(renderer => renderer.sharedMaterial));
        Assert.That(lines.All(line => line.positionCount == 2), Is.True);
        Assert.That(meshes.All(mesh => mesh.vertexCount == 4), Is.True);
        Assert.That(hypercube.faceColors[0], Is.EqualTo(Color.magenta));
    }

    /// <summary>Повторная инициализация воссоздаёт удалённое ребро, сохраняя остальные объекты.</summary>
    [Test]
    public void ReinitializationRecreatesMissingLine()
    {
        Invoke("InitializeGeometry");
        Object.DestroyImmediate(root.transform.Find("Line_0").gameObject);
        Assert.DoesNotThrow(() => Invoke("Update"));
        Invoke("InitializeGeometry");
        Assert.DoesNotThrow(() => Invoke("Update"));
        Assert.That(root.GetComponentsInChildren<LineRenderer>().Length, Is.EqualTo(32));
        Assert.That(root.transform.Find("Line_0").GetComponent<LineRenderer>().positionCount, Is.EqualTo(2));
    }

    /// <summary>Масштаб и поворот родителя не меняют исходный мировой размер и ориентацию проекции.</summary>
    [TestCase(0.01f)]
    [TestCase(1f)]
    [TestCase(2f)]
    public void InitializationPreservesWorldGeometryUnderScaledParent(float parentScale)
    {
        root.transform.localScale = Vector3.one * parentScale;
        root.transform.rotation = Quaternion.Euler(0f, 0f, 30f);
        root.transform.position = new(3f, -2f, 10f);
        hypercube.rotateXY = hypercube.rotateXZ = hypercube.rotateXW = false;
        hypercube.rotateYZ = hypercube.rotateYW = hypercube.rotateZW = false;
        hypercube.wDistance = 5f;
        hypercube.objectScale = 6f;
        hypercube.camera3DDepth = 4f;

        Invoke("Start");
        Invoke("Update");

        // Первая вершина (-1,-1,-1,-1): 4D -> 3D -> 2D даёт (-0.96,-0.96,0).
        Vector3 expectedVertex = root.transform.position + new Vector3(-0.96f, -0.96f, 0f);
        LineRenderer line = root.transform.Find("Line_0").GetComponent<LineRenderer>();
        MeshFilter face = root.transform.Find("Face_0").GetComponent<MeshFilter>();
        Assert.That(Vector3.Distance(line.transform.TransformPoint(line.GetPosition(0)), expectedVertex), Is.LessThan(0.0001f));
        Assert.That(Vector3.Distance(face.transform.TransformPoint(face.sharedMesh.vertices[0]), expectedVertex), Is.LessThan(0.0001f));

        Invoke("InitializeGeometry");
        Invoke("Update");

        foreach (Transform child in root.transform)
        {
            Assert.That(Vector3.Distance(child.lossyScale, Vector3.one), Is.LessThan(0.0001f));
            Assert.That(Quaternion.Angle(child.rotation, Quaternion.identity), Is.LessThan(0.001f));
        }
        Assert.That(Vector3.Distance(line.transform.TransformPoint(line.GetPosition(0)), expectedVertex), Is.LessThan(0.0001f));
        Assert.That(Vector3.Distance(face.transform.TransformPoint(face.sharedMesh.vertices[0]), expectedVertex), Is.LessThan(0.0001f));
    }

    /// <summary>Обновление до завершения подготовки не обращается к пустым массивам.</summary>
    [Test]
    public void UpdateBeforeInitializationDoesNotThrow()
    {
        Assert.DoesNotThrow(() => Invoke("Update"));
        Assert.That(root.transform.childCount, Is.Zero);
    }

    /// <summary>Вызывает закрытый метод жизненного цикла или инициализации компонента.</summary>
    private void Invoke(string name)
    {
        _ = typeof(HypercubeInit).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(hypercube, null);
    }
}
