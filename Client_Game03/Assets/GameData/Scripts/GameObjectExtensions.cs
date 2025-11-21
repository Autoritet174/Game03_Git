using UnityEngine;

namespace Assets.GameData.Scripts
{
    /// <summary>
    /// Предоставляет методы расширения для <see cref="GameObject"/> и <see cref="Object"/>,
    /// обеспечивающие безопасное создание экземпляров (Instantiate).
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Безопасно создает экземпляр указанного <see cref="GameObject"/>.
        /// Если приложение находится в процессе завершения (<see cref="G.IsApplicationQuitting"/>),
        /// возвращает <see langword="null"/> и записывает предупреждение.
        /// </summary>
        /// <param name="original">Исходный <see cref="GameObject"/> для создания экземпляра.</param>
        /// <param name="parent">Родительский <see cref="Transform"/> для нового экземпляра. Значение по умолчанию — <see langword="null"/>.</param>
        /// <returns>
        /// Новый экземпляр <see cref="GameObject"/>, или <see langword="null"/>, если приложение завершается.
        /// </returns>
        public static GameObject SafeInstant(this GameObject original, Transform parent = null)
        {
            if (G.IsApplicationQuitting)
            {
                Debug.LogWarning($"Attempted to'{original.name}' while application is quitting");
                return null;
            }
            return Object.Instantiate(original, parent);
        }

        /// <summary>
        /// Безопасно создает экземпляр указанного <see cref="Object"/>, который является дочерним элементом <typeparamref name="T"/>.
        /// Используется для создания экземпляров <see cref="MonoBehaviour"/> или других <see cref="Object"/> (например, <see cref="ScriptableObject"/>).
        /// Если приложение находится в процессе завершения (<see cref="G.IsApplicationQuitting"/>),
        /// возвращает <see langword="null"/> и записывает предупреждение.
        /// </summary>
        /// <typeparam name="T">Тип <see cref="Object"/>, который нужно создать.</typeparam>
        /// <param name="original">Исходный объект типа <typeparamref name="T"/> для создания экземпляра.</param>
        /// <returns>
        /// Новый экземпляр типа <typeparamref name="T"/>, или <see langword="null"/>, если приложение завершается.
        /// </returns>
        public static T SafeInstant<T>(this T original) where T : Object
        {
            if (G.IsApplicationQuitting)
            {
                Debug.LogWarning($"Attempted to instantiate '{original.name}' while application is quitting");
                return null;
            }
            return Object.Instantiate(original);
        }
    }
}
