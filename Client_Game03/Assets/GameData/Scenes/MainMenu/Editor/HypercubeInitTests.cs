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
        root = new GameObject("Hypercube test");
        hypercube = root.AddComponent<HypercubeInit>();
    }

    /// <summary>Освобождает созданные тестом меши, материалы и объекты.</summary>
    [TearDown]
    public void TearDown()
    {
        foreach (MeshFilter filter in root.GetComponentsInChildren<MeshFilter>())
            Object.DestroyImmediate(filter.sharedMesh);
        foreach (Material material in root.GetComponentsInChildren<Renderer>().Select(renderer => renderer.sharedMaterial).Distinct())
            if (material != null)
                Object.DestroyImmediate(material);
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
        typeof(HypercubeInit).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(hypercube, null);
    }
}
