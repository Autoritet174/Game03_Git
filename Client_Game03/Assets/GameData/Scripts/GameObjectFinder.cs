using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.GameData.Scripts
{
    /// <summary>
    /// Предоставляет методы для поиска игровых объектов на сцене.
    /// </summary>
    public static class GameObjectFinder
    {

        /// <summary>
        /// Возвращает игровой объект из текущей сцены по тегу, находящийся в корне сцены. 
        /// </summary>
        /// <param name="tag">Тег искомого объекта.</param>
        /// <returns>Объект с заданным тегом или null, если не найден.</returns>
        public static GameObject FindByTag(string tag)
        {
            GameObject[] rootObjects = SceneManager
                .GetActiveScene()
                .GetRootGameObjects();

            foreach (GameObject obj in rootObjects)
            {
                if (obj.CompareTag(tag))
                {
                    return obj;
                }
            }

            throw new System.Exception($"Не найден GameObject с тегом {tag}");
        }


        /// <summary>
        /// Рекурсивно ищет игровой объект по имени во всех объектах текущей сцены.
        /// </summary>
        /// <param name="name">Имя искомого объекта.</param>
        /// <returns>Объект с заданным именем или null, если не найден.</returns>
        public static GameObject FindByName(string name)
        {
            GameObject[] rootObjects = SceneManager
                .GetActiveScene()
                .GetRootGameObjects();

            foreach (GameObject root in rootObjects)
            {
                GameObject found = FindInChildrenRecursive(root.transform, name);
                if (found != null)
                {
                    return found;
                }
            }

            throw new System.Exception($"Не найден GameObject с именем {name}");
        }


        /// <summary>
        /// Рекурсивно ищет объект по имени среди всех потомков заданного Transform.
        /// </summary>
        /// <param name="parent">Родительский Transform для начала поиска.</param>
        /// <param name="name">Имя искомого объекта.</param>
        /// <returns>Объект с заданным именем или null.</returns>
        private static GameObject FindInChildrenRecursive(Transform parent, string name)
        {
            if (parent.name == name)
            {
                return parent.gameObject;
            }

            foreach (Transform child in parent)
            {
                GameObject found = FindInChildrenRecursive(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }


        /// <summary>
        /// Ищет объект указанного типа в активной сцене.
        /// Если имя не указано, возвращает первый найденный объект указанного типа.
        /// Если указан startParent, поиск выполняется начиная с его дочерних объектов.
        /// </summary>
        /// <typeparam name="T">Тип компонента, который требуется найти.</typeparam>
        /// <param name="name">Имя искомого объекта (необязательный параметр).</param>
        /// <param name="startParent">Трансформ, откуда начинать поиск (или null для поиска от корня сцены).</param>
        /// <returns>Найденный компонент указанного типа.</returns>
        /// <exception cref="Exception">Если объект не найден.</exception>
        public static T FindByName<T>(string name = null, Transform startParent = null) where T : Component
        {
            if (startParent != null)
            {
                T found = FindInChildrenRecursive<T>(startParent, name);
                if (found != null)
                {
                    return found;
                }
            }
            else
            {
                GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

                foreach (GameObject root in rootObjects)
                {
                    T found = FindInChildrenRecursive<T>(root.transform, name);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            throw new Exception(
                name == null
                    ? $"Не найден объект типа {typeof(T).Name}"
                    : $"Не найден объект типа {typeof(T).Name} с именем {name}"
            );
        }


        /// <summary>
        /// Рекурсивный поиск объекта указанного типа по имени среди дочерних объектов.
        /// Если имя не указано, возвращает первый найденный объект указанного типа.
        /// </summary>
        /// <typeparam name="T">Тип компонента, который требуется найти.</typeparam>
        /// <param name="parent">Родительский трансформ для поиска.</param>
        /// <param name="name">Имя искомого объекта (или null для поиска первого объекта).</param>
        /// <returns>Найденный компонент или null, если объект отсутствует.</returns>
        private static T FindInChildrenRecursive<T>(Transform parent, string name) where T : Component
        {
            if (string.IsNullOrEmpty(name))
            {
                if (parent.TryGetComponent(out T component))
                {
                    return component;
                }
            }
            else if (parent.name == name)
            {
                if (parent.TryGetComponent(out T component))
                {
                    return component;
                }
            }

            foreach (Transform child in parent)
            {
                T found = FindInChildrenRecursive<T>(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

    }

}
