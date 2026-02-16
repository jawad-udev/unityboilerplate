# Unity Boilerplate

A Unity mobile game boilerplate built with **Zenject** (Dependency Injection), **UniRx** (Reactive Extensions), and a clean interface-based service architecture.

## Quick Start

1. Open the project in **Unity 2022+**
2. Go to **Tools → Create GameConfig Asset** to generate the configuration ScriptableObject
3. Edit `Assets/Resources/Config/GameConfig` in the Inspector to set your API URLs and tokens
4. Open `Assets/Scenes/Main.unity` and press Play

## Architecture

This project follows an interface-driven DI architecture. All services are injected via Zenject — no singletons, no static service locators.

For a full guide covering the project structure, dependency injection setup, state machine, UI system, backend networking, and step-by-step instructions for adding new features, see:

**[ARCHITECTURE.md](ARCHITECTURE.md)**

## Tech Stack

| Tool | Purpose |
|------|---------|
| [Zenject](https://github.com/modesttree/Zenject) | Dependency injection |
| [UniRx](https://github.com/neuecc/UniRx) | Reactive data binding |
| [DOTween](http://dotween.demigiant.com/) | Tweening & camera animations |
| [Newtonsoft.Json](https://www.newtonsoft.com/json) | JSON serialization for API layer |
| [NiceVibrations](https://nice-vibrations.moremountains.com/) | Haptic feedback |

## Project Structure

```
Assets/Scripts/
├── Backend/          API clients & networking (UnityWebRequest)
├── CacheManagers/    API / audio / image / video caching
├── Installers/       Zenject bindings (ProjectInstaller, scene installers)
├── Services/         12+ interface-bound services
├── States/           Game state machine (Splash → Menu → Play → GameOver)
├── UI/               Screens & Popup (all inherit GameMonoBehaviour)
├── Scriptables/      GameConfig (env config) + GameElements (prefab registry)
└── Utils/            Helpers, serializer, extensions, enums
```

## Configuration

All tokens, API URLs, and secrets are stored in the `GameConfig` ScriptableObject at `Resources/Config/GameConfig`. Edit it in the Unity Inspector — never hardcode secrets in source files.

To keep secrets out of version control, add to `.gitignore`:

```
Assets/Resources/Config/GameConfig.asset
Assets/Resources/Config/GameConfig.asset.meta
```
