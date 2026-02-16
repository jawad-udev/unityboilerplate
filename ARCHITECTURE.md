# Unity Boilerplate — Architecture Guide

---

## 1. Overview

Unity Boilerplate is a Unity mobile game built on three pillars:

| Pillar | Tool | Role |
|--------|------|------|
| Dependency Injection | **Zenject** | Wires services, UI, and states together without singletons |
| Reactive Data Binding | **UniRx** | Player data and UI auto-update via `ReactiveProperty<T>` |
| Centralized Config | **GameConfig** (ScriptableObject) | All URLs, tokens, and secrets live in one Inspector-editable asset |

**Core rule:** Every service is accessed through an interface (`IPlayerService`, `IAudioService`, etc.) injected via Zenject's `[Inject]` attribute. There are no god objects, no `FindObjectOfType`, and no static service locators.

---

## 2. Project Structure

```
Assets/Scripts/
│
├── Backend/                     ← Networking layer
│   ├── _APIs/                   ← Static API classes (AuthAPI, MediaAPI, PlaylistAPI …)
│   ├── _Models/                 ← Server data models (Backend, Models, AiAudioModels …)
│   ├── RequestDispatcher.cs     ← Executes HTTP via UnityWebRequest coroutines
│   ├── RequestMessage.cs        ← Request builder (URL, headers, body)
│   ├── ResponseMessage.cs       ← Typed response wrapper
│   ├── GameClient.cs            ← Singleton dispatcher + response caching
│   └── GameResponse.cs          ← Raw server response
│
├── CacheManagers/               ← API / audio / image / video caching (session / persistent)
├── Editor/                      ← Editor-only tools (GameConfigCreator)
├── Extensions/                  ← C# extension methods
│
├── Gameplay/
│   └── Managers/
│       └── GameManager.cs       ← Per-session game controller (camera, lives)
│
├── Installers/                  ← Zenject installers
│   ├── ProjectInstaller.cs      ← Root bindings (all services, UI, signals)
│   ├── GameSceneInstaller.cs    ← Game-scene bindings
│   ├── MainSceneInstaller.cs    ← Main-scene bindings
│   └── GameDifficultyInstaller.cs ← Difficulty settings
│
├── Models/                      ← Data models (Player, Difficulty)
│
├── Scriptables/                 ← ScriptableObjects
│   ├── GameConfig.cs            ← Tokens, URLs, version — the "env file"
│   ├── GameElements.cs          ← Prefab registry for services & UI
│   ├── GameSceneElements.cs     ← Game-scene prefab references
│   └── MainSceneElements.cs     ← Main-scene prefab references
│
├── Services/                    ← All game services
│   ├── Interfaces/              ← 12 service interfaces
│   ├── PlayerService.cs         ← Player data + file-based persistence
│   ├── GameService.cs           ← State machine + game lifecycle
│   ├── UIService.cs             ← Screen/popup routing
│   ├── AudioService.cs          ← SFX and music
│   ├── ScoreService.cs          ← Scoring logic
│   ├── CameraService.cs         ← Zoom, shake, viewport
│   ├── SceneService.cs          ← Scene loading
│   ├── InputService.cs          ← Input handling
│   ├── BackLogService.cs        ← UI back-navigation stack
│   ├── VibrationService.cs      ← Haptic feedback
│   ├── EffectService.cs         ← Particle effects
│   ├── ApiService.cs            ← Server API orchestration
│   ├── ColorService.cs          ← Color/theme management
│   ├── UserService.cs           ← User-related logic
│   └── Services.cs              ← AppInitializer (bootstrap only)
│
├── Signals/                     ← Zenject signals (decoupled events)
│   └── GameSignals.cs
│
├── States/                      ← Game state machine
│   ├── _StatesBase.cs           ← StateBase abstract class
│   ├── SplashState.cs
│   ├── MenuState.cs
│   ├── GamePlayState.cs
│   ├── GameOverState.cs
│   └── GamePauseState.cs
│
├── UI/
│   ├── Screens/                 ← Full-screen views (Splash, Home, GamePlay)
│   └── Popup/                   ← Modals (Settings, Pause, Win, Lose, Fail, Profile, Common)
│
└── Utils/                       ← Helpers (GameMonoBehaviour, GameSerializer, CameraShake, SafeArea …)
```

---

## 3. Dependency Injection (Zenject)

### 3.1 Where Bindings Live

All bindings are in **`ProjectInstaller.cs`**, attached to the `ProjectContext` prefab at `Resources/ProjectContext`. This makes them available across every scene.

### 3.2 Three Binding Patterns

**Plain C# services** (no MonoBehaviour needed):

```csharp
Container.Bind<IPlayerService>().To<PlayerService>().AsSingle().NonLazy();
```

**MonoBehaviour services from a prefab** (need a real GameObject):

```csharp
Container.BindInterfacesAndSelfTo<GameService>()
    .FromComponentInNewPrefab(gameElements.gameService)
    .AsSingle().NonLazy();
```

**MonoBehaviour services from a new empty GameObject**:

```csharp
Container.BindInterfacesAndSelfTo<InputService>()
    .FromNewComponentOnNewGameObject()
    .WithGameObjectName("InputService")
    .AsSingle().NonLazy();
```

### 3.3 NonLazy vs Lazy

| Modifier | Behaviour | Use For |
|----------|-----------|---------|
| `.NonLazy()` | Created immediately at startup | Core services (GameService, PlayerService, AudioService) |
| *(omitted)* | Created the first time something injects it | UI screens and popups |

### 3.4 What Gets Bound

```
GameConfig             ← singleton ScriptableObject (tokens, URLs)
GameElements           ← singleton ScriptableObject (prefab registry)

IPlayerService         → PlayerService          (NonLazy)
IUIService             → UIService              (NonLazy)
IScoreService          → ScoreService           (NonLazy)
IVibrationService      → VibrationService
ICameraService         → CameraService

GameService / IGameService       ← from prefab  (NonLazy)
AudioService / IAudioService     ← from prefab  (NonLazy)
SceneService / ISceneService     ← from prefab  (NonLazy)
InputService / IInputService     ← new GO       (NonLazy)
BackLogService / IBackLogService ← new GO       (NonLazy)
EffectService / IEffectService   ← new GO
ApiService / IApiService         ← new GO       (NonLazy)

Canvas                 ← from globalCanvasPrefab     (NonLazy)
UiOrganizer            ← plain class

SplashScreen, HomeScreen, GamePlayScreen         ← lazy, from prefab
CommonPopup, GameFailPopup, GameWinPopup, etc.   ← lazy, from prefab

ColorService           ← color/theme management
UserService            ← user-related logic

ScoreUpdateSignal      ← Zenject signal
```

---

## 4. Service Interfaces

Every service is accessed through its interface. The interfaces live in `Services/Interfaces/`.

| Interface | What It Does |
|-----------|-------------|
| `IPlayerService` | Player data CRUD, reactive properties, file-based save/load |
| `IGameService` | State machine transitions, game spawning, win/lose flow |
| `IUIService` | Activate screens by enum, access any screen/popup reference |
| `IAudioService` | Play SFX, toggle music, control volume |
| `IScoreService` | Track and update current score + high score |
| `ICameraService` | Camera zoom, shake, viewport math |
| `ISceneService` | Load/unload scenes |
| `IInputService` | Input detection |
| `IBackLogService` | UI navigation stack (back button support) |
| `IVibrationService` | Haptic feedback |
| `IEffectService` | Spawn particle effects |
| `IApiService` | Orchestrate server API calls |

---

## 5. How to Use a Service

### 5.1 In Any MonoBehaviour

```csharp
using Zenject;

public class MyScreen : GameMonoBehaviour
{
    [Inject] private IPlayerService _playerService;
    [Inject] private IAudioService _audioService;

    private void Start()
    {
        string name = _playerService.GetPlayerName();
        _audioService.PlayUIClick();
    }
}
```

### 5.2 In a Plain C# Class (Constructor Injection)

```csharp
public class MyService : IMyService
{
    private readonly IPlayerService _playerService;

    public MyService(IPlayerService playerService)
    {
        _playerService = playerService;
    }
}
```

### 5.3 In a Game State

```csharp
public class MyState : StateBase
{
    [Inject] private IUIService _uiService;

    public override void OnActivate()
    {
        _uiService.ActivateUIScreen(Screens.HOME);
    }

    public override void OnDeactivate() { }
    public override void OnUpdate() { }
}
```

### 5.4 What NOT to Do

```csharp
// ❌ Old singleton pattern — removed
Services.PlayerService.GetPlayerName();

// ❌ FindObjectOfType
FindObjectOfType<AudioService>().PlayUIClick();

// ✅ Always use [Inject]
[Inject] private IAudioService _audioService;
```

---

## 6. State Machine

`GameService` manages game flow through a dictionary of `StateBase` states. Each state is a MonoBehaviour placed as a child of the GameService prefab.

### 6.1 Available States

| State | When Active |
|-------|------------|
| `SplashState` | App launch → splash animation |
| `MenuState` | Main menu |
| `GamePlayState` | Active gameplay |
| `GameOverState` | Win or lose results |
| `GamePauseState` | Pause overlay |

### 6.2 Switching States

```csharp
[Inject] private IGameService _gameService;

_gameService.SetState<MenuState>();        // compile-time safe
_gameService.SetState(typeof(MenuState));  // runtime type
```

### 6.3 Creating a New State

1. Create a class extending `StateBase`:

```csharp
using Zenject;

public class TutorialState : StateBase
{
    [Inject] private IUIService _uiService;

    public override void OnActivate()  { _uiService.ActivateUIScreen(Screens.HOME); }
    public override void OnDeactivate() { }
    public override void OnUpdate()     { }
}
```

2. Add it as a **child GameObject** of the GameService prefab in the scene/prefab.
3. Done — `GameService` auto-discovers it at startup.

---

## 7. UI System

### 7.1 Base Class: `GameMonoBehaviour`

All screens and popups inherit from `GameMonoBehaviour`. It provides:

- `Show()` — activates the view and pushes it onto the back-navigation stack.
- `Show(BacklogType)` — controls stack behaviour.
- `Hide()` — deactivates the view.

### 7.2 BacklogType Options

| Value | Effect |
|-------|--------|
| `DisablePreviousScreen` | Hides current top before showing new one *(default)* |
| `KeepPreviousScreen` | Keeps previous visible (layered) |
| `ClearTop` | Wipes the entire navigation stack |
| `Ignore` | Doesn't touch the stack (overlays) |
| `RemovePreviousScreen` | Removes previous from stack on hide |

### 7.3 Showing UI via UIService

```csharp
[Inject] private IUIService _uiService;

// By enum (recommended)
_uiService.ActivateUIScreen(Screens.HOME);
_uiService.ActivateUIPopups(Popups.SETTINGS);

// Direct reference
_uiService.HomeScreen.Show();
_uiService.SettingsPopup.Show(BacklogType.Ignore);
```

### 7.4 Reactive UI Binding

Use UniRx to bind player data to labels — they update automatically:

```csharp
[Inject] private IPlayerService _playerService;

[SerializeField] private Text coinsLabel;

private void OnEnable()
{
    _playerService.Coins
        .Subscribe(c => coinsLabel.text = c.ToString())
        .AddTo(this);  // auto-disposed when MonoBehaviour is destroyed
}
```

### 7.5 Adding a New Screen

| Step | Action |
|------|--------|
| 1 | Create a prefab with your UI layout |
| 2 | Write a script inheriting `GameMonoBehaviour` with `[Inject]` fields |
| 3 | Add a reference field to `GameElements.cs` |
| 4 | Add a new value to the `Screens` enum in `GameEnums.cs` |
| 5 | Add a binding in `ProjectInstaller.cs`: `Container.BindInterfacesAndSelfTo<MyScreen>().FromComponentInNewPrefab(gameElements.myScreen).AsSingle();` |
| 6 | Add a case in `UIService.ActivateUIScreen()` |

Popups follow the same steps, using the `Popups` enum and `ActivateUIPopups()`.

---

## 8. GameConfig — Centralized Configuration

All environment-specific values are stored in a **ScriptableObject** instead of being hardcoded in source files.

### 8.1 Asset Location

```
Assets/Resources/Config/GameConfig.asset
```

### 8.2 Fields

| Field | Purpose | Read By |
|-------|---------|---------|
| `hostUrl` | Main REST API base URL | `GameClient`, all API classes |
| `hostAudioUrl` | Audio/Speech API base URL | `GameClient` |
| `cloudfrontUrl` | CloudFront CDN base URL | `GameClient` |
| `appToken` | `X-App-Token` header for authenticated calls | API classes via `GameClient.AppToken` |
| `defaultRequestAppToken` | Default `X-App-Token` on every request | `RequestMessage._defaultHeaders` |
| `gameVersionCode` | `X-Forwarded-Version` header | `RequestMessage._defaultHeaders` |

### 8.3 Creating the Asset (First Time)

In Unity → **Tools → Create GameConfig Asset**

Or: **Assets → Create → ScriptableObjects → GameConfig**, then move to `Resources/Config/`.

### 8.4 Accessing in Code

```csharp
// Static accessor (works anywhere)
string url = GameConfig.Instance.hostUrl;

// Or inject via Zenject (preferred in services)
[Inject] private GameConfig _config;
```

### 8.5 Keeping Secrets Out of Git

Add to `.gitignore`:

```
Assets/Resources/Config/GameConfig.asset
Assets/Resources/Config/GameConfig.asset.meta
```

Check in a template with placeholder values for teammates.

---

## 9. Backend / Networking

### 9.1 Request Flow

```
Static API class         GameClient                  RequestDispatcher
(AuthAPI, MediaAPI)  →   (cache check, dispatch)  →  (UnityWebRequest coroutine)
                                                           ↓
callback ← ResponseMessage<T> ← GameResponse ← HTTP response
```

### 9.2 Making an API Call

```csharp
using Backend;

AuthAPI.Login("user@email.com", "password", response =>
{
    if (response._status)
    {
        Debug.Log("Welcome " + response._entity.user.email);
    }
    else
    {
        Debug.LogError("Error: " + response._error);
    }
});
```

### 9.3 Authentication Flow

1. **App token** — automatically injected into every request header via `RequestMessage._defaultHeaders` (read from `GameConfig`).
2. **User bearer token** — API classes read `GameClient.instance.AccessToken`. This is kept in sync automatically: when you set `PlayerService.AccessToken`, it writes to `GameClient.AccessToken` too.

### 9.4 Adding a New API Endpoint

Create a static class in `Backend/_APIs/`:

```csharp
using System;

namespace Backend
{
    public static class MyNewAPI
    {
        private static void AddAuth(RequestMessage req)
        {
            req._headers = RequestMessage._defaultHeaders;
            req._headers.Add("X-APP-Token", GameClient.instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.instance.AccessToken);
        }

        public static void GetItem(string id, Action<ResponseMessage<ItemResponse>> listener)
        {
            string url = GameClient.instance._hostUrl + $"api/v1/items/{id}/";
            var req = new RequestMessage
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = url
            };
            AddAuth(req);
            GameClient.instance.DispatchRequest(req, listener);
        }
    }
}
```

Add the response model in `Backend/_Models/`:

```csharp
namespace Backend
{
    [System.Serializable]
    public class ItemResponse
    {
        public string id;
        public string name;
    }
}
```

---

## 10. Player Data Persistence

### 10.1 How It Works

`PlayerService` stores player data as JSON in a file, not PlayerPrefs.

| Detail | Value |
|--------|-------|
| Save path | `Application.persistentDataPath/player_data.json` |
| Auto-save | Every 5 seconds (only if data changed) |
| Quit save | Flushes on `OnApplicationQuit` |
| Legacy migration | Migrates old `PlayerPrefs` data on first launch |

### 10.2 Reactive Properties

Player data fields are `ReactiveProperty<T>` — UI subscribes once and updates automatically:

```csharp
[Inject] private IPlayerService _playerService;

// Read
int coins = _playerService.GetPlayerCoins();

// Write (auto-persisted on next 5s flush)
_playerService.SetCoins(100);

// Subscribe for live updates
_playerService.Coins
    .Subscribe(c => coinsLabel.text = c.ToString())
    .AddTo(this);
```

---

## 11. Signals (Event Bus)

Zenject Signals allow decoupled cross-service communication.

### 11.1 Define

```csharp
// Signals/GameSignals.cs
public class ScoreUpdateSignal
{
    public float score;
    public ScoreUpdateSignal(int score) { this.score = score; }
}
```

### 11.2 Register

```csharp
// ProjectInstaller.cs
Container.DeclareSignal<ScoreUpdateSignal>();
```

### 11.3 Fire

```csharp
[Inject] private SignalBus _signalBus;

_signalBus.Fire(new ScoreUpdateSignal(500));
```

### 11.4 Listen

```csharp
[Inject] private SignalBus _signalBus;

void Start()   => _signalBus.Subscribe<ScoreUpdateSignal>(OnScore);
void OnDestroy() => _signalBus.Unsubscribe<ScoreUpdateSignal>(OnScore);

void OnScore(ScoreUpdateSignal s) => Debug.Log("Score: " + s.score);
```

---

## 12. Adding a New Service (Step by Step)

### 12.1 Plain C# Service

| Step | File | Code |
|------|------|------|
| 1. Interface | `Services/Interfaces/IMyService.cs` | `public interface IMyService { void DoWork(); }` |
| 2. Implementation | `Services/MyService.cs` | `public class MyService : IMyService { ... }` |
| 3. Binding | `Installers/ProjectInstaller.cs` | `Container.Bind<IMyService>().To<MyService>().AsSingle();` |
| 4. Usage | Any class | `[Inject] private IMyService _myService;` |

### 12.2 MonoBehaviour Service

Same as above, but use one of these bindings:

```csharp
// From prefab (add prefab ref to GameElements first)
Container.BindInterfacesAndSelfTo<MyMonoService>()
    .FromComponentInNewPrefab(gameElements.myMonoService)
    .AsSingle().NonLazy();

// From new empty GameObject
Container.BindInterfacesAndSelfTo<MyMonoService>()
    .FromNewComponentOnNewGameObject()
    .WithGameObjectName("MyMonoService")
    .AsSingle().NonLazy();
```

---

## 13. Naming Conventions

| Thing | Pattern | Example |
|-------|---------|---------|
| Service interface | `I` + PascalCase + `Service` | `IPlayerService` |
| Service class | PascalCase + `Service` | `PlayerService` |
| Injected field | `_` + camelCase | `_playerService` |
| Game state | PascalCase + `State` | `GamePlayState` |
| Screen | PascalCase + `Screen` | `HomeScreen` |
| Popup | PascalCase + `Popup` | `SettingsPopup` |
| Signal | PascalCase + `Signal` | `ScoreUpdateSignal` |
| Enum | PascalCase | `GameStatus`, `BacklogType` |

---

## 14. Key Enums Reference

```csharp
enum Screens   { SPLASH, HOME, PLAY }
enum Popups    { PROFILE, SETTINGS, PAUSE, WIN, LOSE, FAIL }
enum GameMode  { SinglePlayer, MultiPlayer }
enum GameStatus { TOSTART, ONGOING, PAUSED, WON, LOST }
enum BacklogType { DisablePreviousScreen, KeepPreviousScreen, ClearTop, Ignore, RemovePreviousScreen }
enum UIType    { Screen, Popup }
```

---

## 15. Troubleshooting

| Problem | Cause | Fix |
|---------|-------|-----|
| `GameConfig asset not found` | Asset doesn't exist yet | Unity → **Tools → Create GameConfig Asset** |
| `ZenjectException: Unable to resolve IMyService` | Not bound in installer | Add binding in `ProjectInstaller.cs` |
| `[Inject] field is null` | Object created outside Zenject | Bind it in installer, or add `ZenjectBinding` component, or call `Container.InjectGameObject()` |
| Buttons don't work on a screen | Injection not happening | Make sure the prefab is bound in `ProjectInstaller.cs` and accessed through `IUIService` |
| Player data not saving | File system issue | Check `Application.persistentDataPath` is writable; data file is `player_data.json` |
| API calls fail with 401 | Token not set | Ensure `PlayerService.AccessToken` is set after login (it syncs to `GameClient` automatically) |

---

*Last updated: February 2026*
