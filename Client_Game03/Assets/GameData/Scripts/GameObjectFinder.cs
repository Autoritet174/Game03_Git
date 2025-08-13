using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        /// Рекурсивно ищет компонент TextMeshProUGUI по имени объекта.
        /// </summary>
        /// <param name="name">Имя объекта, содержащего компонент TextMeshProUGUI.</param>
        /// <returns>TextMeshProUGUI-компонент или null, если не найден.</returns>
        public static TextMeshProUGUI FindTextMeshProUGUIByName(string name)
        {
            GameObject[] rootObjects = SceneManager
                .GetActiveScene()
                .GetRootGameObjects();

            foreach (GameObject root in rootObjects)
            {
                TextMeshProUGUI found = FindTMPInChildrenRecursive(root.transform, name);
                if (found != null)
                {
                    return found;
                }
            }

            throw new System.Exception($"Не найден TextMeshProUGUI с именем {name}");
        }

        /// <summary>
        /// Рекурсивно ищет компонент TextMeshProUGUI среди потомков по имени объекта.
        /// </summary>
        /// <param name="parent">Родительский Transform.</param>
        /// <param name="name">Имя искомого объекта.</param>
        /// <returns>TextMeshProUGUI-компонент или null.</returns>
        private static TextMeshProUGUI FindTMPInChildrenRecursive(Transform parent, string name)
        {
            if (parent.name == name)
            {
                return parent.GetComponent<TextMeshProUGUI>();
            }

            foreach (Transform child in parent)
            {
                TextMeshProUGUI found = FindTMPInChildrenRecursive(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }



        /// <summary>
        /// Рекурсивно ищет компонент TMP_InputField по имени объекта.
        /// </summary>
        /// <param name="name">Имя объекта, содержащего компонент TMP_InputField.</param>
        /// <returns>TMP_InputField-компонент или null, если не найден.</returns>
        public static TMP_InputField FindTMPInputFieldByName(string name)
        {
            GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager
                .GetActiveScene()
                .GetRootGameObjects();

            foreach (GameObject root in rootObjects)
            {
                TMP_InputField found = FindTMPInputFieldInChildrenRecursive(root.transform, name);
                if (found != null)
                {
                    return found;
                }
            }

            throw new System.Exception($"Не найден TMP_InputField с именем {name}");
        }

        /// <summary>
        /// Рекурсивно ищет компонент TMP_InputField среди потомков по имени объекта.
        /// </summary>
        /// <param name="parent">Родительский Transform.</param>
        /// <param name="name">Имя искомого объекта.</param>
        /// <returns>TMP_InputField-компонент или null.</returns>
        private static TMP_InputField FindTMPInputFieldInChildrenRecursive(Transform parent, string name)
        {
            if (parent.name == name)
            {
                return parent.GetComponent<TMP_InputField>();
            }

            foreach (Transform child in parent)
            {
                TMP_InputField found = FindTMPInputFieldInChildrenRecursive(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }


        /// <summary>
        /// Рекурсивно ищет компонент Button по имени объекта.
        /// </summary>
        /// <param name="name">Имя объекта, содержащего компонент Button.</param>
        /// <returns>Button-компонент или null, если не найден.</returns>
        public static Button FindButtonByName(string name)
        {
            GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager
                .GetActiveScene()
                .GetRootGameObjects();

            foreach (GameObject root in rootObjects)
            {
                Button found = FindButtonInChildrenRecursive(root.transform, name);
                if (found != null)
                {
                    return found;
                }
            }

            throw new System.Exception($"Не найден Button с именем {name}");
        }

        /// <summary>
        /// Рекурсивно ищет компонент Button среди потомков по имени объекта.
        /// </summary>
        /// <param name="parent">Родительский Transform.</param>
        /// <param name="name">Имя искомого объекта.</param>
        /// <returns>Button-компонент или null.</returns>
        private static Button FindButtonInChildrenRecursive(Transform parent, string name)
        {
            if (parent.name == name)
            {
                return parent.GetComponent<Button>();
            }

            foreach (Transform child in parent)
            {
                Button found = FindButtonInChildrenRecursive(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }

}
