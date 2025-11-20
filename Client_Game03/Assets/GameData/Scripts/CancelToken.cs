using System;
using System.Collections.Generic;
using System.Threading;

namespace Assets.GameData.Scripts
{
    /// <summary>
    /// Статический класс для централизованного управления токенами отмены.
    /// Каждый токен связан с глобальной отменой выхода из приложения и таймаутом.
    /// </summary>
    internal class CancelToken
    {
        // Глобальный источник отмены при выходе из приложения.
        private static readonly CancellationTokenSource _globalQuitCts = new();

        // Словарь для хранения активных связанных токенов: (Name, (TimeoutCTS, LinkedCTS)).
        private static readonly Dictionary<string, (CancellationTokenSource TimeoutCts, CancellationTokenSource LinkedCts)> _activeTokens = new();
        private static readonly object _lock = new();

        /// <summary>
        /// Создает новый токен отмены, связанный с глобальной отменой и таймаутом.
        /// Если токен с таким именем уже существует, он отменяется и удаляется.
        /// </summary>
        /// <param name="name">Уникальное имя токена.</param>
        /// <param name="sec">Время таймаута в секундах, после которого токен будет отменен.</param>
        /// <returns>Новый связанный токен отмены.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если время ожидания меньше или равно нулю.</exception>

        public static CancellationToken Create(string name, int sec = 30)
        {
            // Проверка валидности входных данных
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Имя токена не может быть пустым.", nameof(name));
            }
            if (sec <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sec), "Время ожидания должно быть больше нуля.");
            }


            if (G.IsApplicationQuitting)
            {
                // Если какой то Task начал выполняться после начала процесса выхода из приложения, то он запросит получение токена и сразу получит токен с флагом отмены
                CancellationTokenSource ctsTemp = new();
                ctsTemp.Cancel();
                return ctsTemp.Token;
            }

            // Создание источника таймаута
            CancellationTokenSource timeoutCts = new(TimeSpan.FromSeconds(sec));

            // Связывание: (1) таймаут, (2) глобальный выход
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, _globalQuitCts.Token);

            lock (_lock)
            {
                if (_activeTokens.TryGetValue(name, out (CancellationTokenSource TimeoutCts, CancellationTokenSource LinkedCts) oldSources))
                {
                    // Отмена и удаление старых источников
                    try
                    {
                        oldSources.LinkedCts.Cancel();
                        oldSources.TimeoutCts.Cancel();
                    }
                    catch { }

                    // Обеспечение Dispose для всех компонентов старого токена
                    try
                    {
                        oldSources.LinkedCts.Dispose();
                        oldSources.TimeoutCts.Dispose();
                    }
                    catch { }

                    _ = _activeTokens.Remove(name);
                }

                // Добавление новых связанных источников в словарь
                _activeTokens.Add(name, (timeoutCts, linkedCts));
            }

            return linkedCts.Token;
        }

        /// <summary>
        /// Отменяет глобальный токен выхода и все активные локальные токены.
        /// Должен быть вызван при завершении работы приложения.
        /// </summary>
        public static void CancelAllTokens()
        {
            _globalQuitCts.Cancel();

            lock (_lock)
            {
                if (_activeTokens.Count > 0)
                {
                    foreach (KeyValuePair<string, (CancellationTokenSource TimeoutCts, CancellationTokenSource LinkedCts)> item in _activeTokens)
                    {
                        try
                        {
                            item.Value.LinkedCts.Cancel();
                        }
                        catch { }

                        try
                        {
                            item.Value.TimeoutCts.Cancel();
                        }
                        catch { }

                        try
                        {
                            item.Value.LinkedCts.Dispose();
                        }
                        catch { }

                        try
                        {
                            item.Value.TimeoutCts.Dispose();
                        }
                        catch { }

                    }
                    _activeTokens.Clear();
                }
            }
        }
    }
}
