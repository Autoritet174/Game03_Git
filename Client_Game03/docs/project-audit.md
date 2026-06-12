# Аудит Unity-проекта Game03 (Client_Game03)

Дата: 11 июня 2026  
Область: ~77 C#-скриптов, 34 сцены, 23 prefab'а, YAML-ассеты и конфигурация.

---

## Общая картина

Компактный Unity 6 клиент (URP, Addressables, UniTask) с scene-based архитектурой. Игровая логика вынесена в `Game03Client.dll` / `General.dll`, Unity-слой — UI и сцены. Идёт переход на паттерн **prefab + context interface** (Collection, SelectBattlefield), но старые паттерны (static singleton, `FindByName`) ещё широко используются.

**Сцены в билде:** Auth → AllHeroes → Battlefield → Collection → MainMenu → SelectBattlefield.

---

## Критично (исправить в первую очередь)

### 3. `async void` в lifecycle и UI (7 мест)

| Файл | Метод |
|------|-------|
| `CollectionSceneInitializator.cs` | `async void Awake()` (без `await`), `async void Start()` |
| `BattlefieldSceneInitializator.cs` | `async void Start()` |
| `AuthSceneInitializator.cs` | `async void Start()` |
| `AllHeroes.cs` | `async void Start()` |
| `ButtonClose_Click_EndBattle.cs` | `async void OnClick()` |
| `GameExitHandler.cs` | `async void ExitGame()` |

**Риск:** необработанные исключения «тихо» роняют сцену. Заменить на `UniTaskVoid` + `.Forget()` с обработкой или `async UniTask` с явным await.

### 4. Sync-over-async при старте

- `G.cs` — `Addressables.LoadAssetAsync` + `WaitForCompletion()` при `BeforeSceneLoad`
- `PanelSelectedHero/Equipment` — `Hide().GetAwaiter().GetResult()` в `Start`/`Awake`
- `GameMessage.cs` — `WaitForCompletion()` при показе

**Риск:** фризы при загрузке, deadlock в edge cases. Перевести на чистый async pipeline через UniTask.

---

## Высокий приоритет

### NullReference и хрупкая инициализация

- **`CollectionSceneInitializator.OnResized()`** — вызывает static-инстансы без null-check; при частичном сбое `Awake` возможен каскад NRE
- **`PanelCollectionViewer__prefab__scriptMB.cs:34`** — `FindByName("Content", parent).transform` без проверки (двухаргументный overload возвращает `null`, не бросает)
- **Hero/Equipment initializators** — `FindByName(...).GetComponent<>()` без проверки компонента
- **`SelectBattlefieldSceneInitializator.Instance.OnResized()`** — без `?.`, хотя в context-адаптере уже есть проверка

### Несогласованный `GameObjectFinder`

- `FindByName(string)` без parent → **throw Exception**
- `FindByName(string, Transform)` → **return null**

Один API, два поведения — частый источник багов.

### Hardcoded URL сервера

`G.cs`: `General.Url.Init("https://localhost:7227")` — нет конфигурации под staging/production.

### Refresh token в PlayerPrefs

`SecureStorageProvider.cs` — DPAPI только Windows/Editor; на mobile — `NotSupportedException`. Для mobile prod нужен Keychain/Keystore.

---

## Производительность

### Тяжёлые `Update()`

| Файл | Проблема |
|------|----------|
| `HypercubeInit.cs` | Каждый кадр: 24 mesh с пересозданием vertices/triangles, 32 LineRenderer |
| `BattlefieldSceneInitializator.cs` | LINQ `FirstOrDefault` в Update, foreach по юнитам, `Debug.Log` в hot path |
| `Health.cs` | `DateTime.Now` каждый кадр вместо `Time.deltaTime` |
| `PanelReconnecting__prefab__scriptMB.cs` | String interpolation в TMP каждый кадр |

### LINQ в UI-path (не каждый кадр, но тяжело)

- `PanelSelectedHero__prefab__scriptMB.cs` — 6× `SelectMany/Where/Sum` при каждом `Show()`
- `PanelCollectionViewer__prefab__scriptMB.cs` — `.Where().OrderByDescending()` при построении коллекции

### Resize polling в 8+ местах

Одинаковый паттерн `Update()` → проверка `Screen.width/height` → `OnResized()` в initializator'ах и prefab'ах. Кандидат на общий `ScreenResizeNotifier` или один central listener.

---

## Архитектура и организация

### Что уже хорошо

- Prefab + context interface для Collection UI
- UniTask вместо coroutines в большинстве мест
- Addressables как основной путь загрузки (Resources.Load — только локализация)
- `HttpClient` singleton в `GameServerPinger`
- Битых `m_Script: {fileID: 0}` в YAML **не найдено**

### Что стоит улучшить

**1. Дублирование scene initializators**  
5+ классов с одинаковым шаблоном: FindByName → static Instance → Update resize. Вынести базовый класс или composable helper.

**2. Namespace inconsistency**  
Часть классов в `Assets.GameData.*`, часть в global namespace (`CollectionSceneInitializator`, prefab-скрипты). Опечатка: `BattleField` vs `Battlefield`.

**3. Prefab-скрипты разбросаны**  
`GameData/Prefabs/` и `GameData/Scenes/*/prefabs/` — правило `__prefab` применяется не везде (9 prefab без суффикса, `_Prefab` vs `__prefab`).

**4. Static singleton + context pattern одновременно**  
Prefab'ы decouple через context, но scene-level код всё ещё завязан на `CollectionSceneInitializator.PanelCollectionViewerInstance` — смешение двух подходов.

**5. God-классы (400+ строк)**  
- `PanelSelectedEquipment__prefab__scriptMB.cs` (436)
- `HypercubeInit.cs` (395)
- `PanelSelectedHero__prefab__scriptMB.cs` (329)
- `PanelGroupDivider__prefab__script.cs` (326)

Кандидаты на декомпозицию: UI init / resize / business logic.

**6. Мёртвый код**  
- `InputManager.cs` — ~499 строк, ~95% закомментировано
- `BattlefieldSceneInitializator` — пустые stub'ы `Ability1OnClickAsync` … `Ability3OnClickAsync`
- `SpriteGenerator.cs` — полностью закомментирован

---

## Ассеты и сцены

### Заготовки без логики

- `GameModesScene.unity` — только Main Camera, нет initializator, не в build
- `LoadingScreenScene.unity` — то же самое

Либо довести до конца, либо удалить.

### Мусор в репозитории

- **`Assets/_Recovery/`** — 24 auto-recovery сцены после крашей Unity
- **`Assets/Packages/`** — NuGet-артефакты (~500+ файлов), обычно не коммитят
- **`YughuesFreeMetalMaterials/`** — сторонний pack с `.unitypackage`

### Устаревшие `m_EditorClassIdentifier` в YAML

Классы переименованы, guid валиден, но в YAML старые имена (`NewMonoBehaviourScript`, `SelectDungeonScene` и т.д.). Пересохранение в Unity почистит.

### Editor-ловушка

`PanelCollectionTopButtonsPrefabCreator` ищет TopButtons **на корне сцены**, но сейчас он **вложен** в `PanelCollection__prefab`. При удалении standalone-prefab автосоздание молча провалится.

---

## Безопасность и git

| Находка | Severity |
|---------|----------|
| Hardcoded dev credentials в Auth | **High** (prod) |
| Refresh token в PlayerPrefs (Win only) | Medium |
| `LogRefreshToken()` в AuthHelper — логирует hash токена | Low (закомментировано) |
| Предсказуемые ключи `"win_sec_{key}"` в SecureStorage | Low |

### `.gitignore` пробелы

- `.idea/` не игнорируется глобально (есть на диске)
- `Assets/GameData/Logs/` (runtime-логи от `LoggerException`) не в ignore
- `*.csproj` / `*.sln` закомментированы — csproj коммитятся (осознанный выбор, но шум в diff)

`Library/`, `Temp/`, корневой `Logs/` — **не в git**, ок.

---

## Event lifecycle

Подписки `onClick.AddListener` без `RemoveListener` в `OnDestroy`:

- `PanelGroupDivider__prefab__script.cs`
- `AllHeroes.cs` (динамические hero viewer)
- `PanelReconnecting__prefab__scriptMB.cs` (`DontDestroyOnLoad`)

Единственный `OnDestroy` cleanup — `MainThreadDispatcher.cs`.

---

## Зависимости и инфраструктура

**UPM:** UniTask, Addressables 2.9.1, URP 17.3, Input System, Ads/Purchasing/Analytics (возможно не используются — стоит проверить).

**0 `.asmdef` файлов** — вся логика в `Assembly-CSharp`:
- долгая перекомпиляция при любом изменении
- `AddressableNamesGenerator` попал в runtime из-за отсутствия asmdef + неправильного расположения

**Рекомендуемая структура asmdef:**
- `Game03.Runtime` — Scripts + Scenes logic
- `Game03.Editor` — Editor/, Redactor/, references Runtime

---

## Hardcoded имена объектов

~80+ вызовов `GameObjectFinder.FindByName` с именами вида `"PanelCollection (id=jcxwa01g)"`. Работает, но хрупко при переименовании в сцене. Частично решено константами в prefab-скриптах; в initializator'ах — строки inline.

Альтернативы (на будущее): `[SerializeField]` ссылки, nested prefab references, ScriptableObject registry.

---

## Приоритетный roadmap улучшений

| # | Задача | Effort | Impact |
|---|--------|--------|--------|
| 1 | Убрать/обернуть dev credentials в Auth | Low | High |
| 2 | Перенести `AddressableNamesGenerator` в Editor | Low | High (build) |
| 3 | Заменить `async void` на UniTask lifecycle | Medium | High |
| 4 | Null-safe в initializator'ах + унифицировать GameObjectFinder | Medium | High |
| 5 | Убрать sync-over-async (`WaitForCompletion`, `GetResult`) | Medium | Medium |
| 6 | Оптимизировать `HypercubeInit` и Battlefield Update | Medium | Medium |
| 7 | Базовый класс для scene resize initializators | Medium | Medium |
| 8 | Очистить `_Recovery`, orphan-сцены, мёртвый код | Low | Low |
| 9 | Ввести asmdef (Runtime/Editor) | Medium | Medium (DX) |
| 10 | Унифицировать naming prefab + namespaces | High | Low (DX) |

---

## Итог

Проект в рабочем состоянии с понятной архитектурой для своего масштаба. Главные риски — **dev credentials в Auth**, **Editor-код в runtime-сборке**, **async void + sync-over-async**, **хрупкая инициализация через FindByName/static singleton**. Архитектурно правильное направление — prefab + context; имеет смысл довести его до остальных сцен и постепенно убрать прямые ссылки на scene singleton'ы.
