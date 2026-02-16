using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller<ProjectInstaller>
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        GameElements gameElements = Resources.Load<GameElements>("UI/SO/GameElements");
        Container.Bind<GameElements>().FromInstance(gameElements);

        // App configuration (tokens, URLs) — edit the asset at Resources/Config/AppConfig
        Container.Bind<GameConfig>().FromInstance(GameConfig.Instance).AsSingle();

        // Core Services (interface-bound, proper DI)
        Container.Bind<IPlayerService>().To<PlayerService>().AsSingle().NonLazy();
        Container.Bind<IUIService>().To<UIService>().AsSingle().NonLazy();
        Container.Bind<IVibrationService>().To<VibrationService>().AsSingle();
        Container.Bind<IScoreService>().To<ScoreService>().AsSingle().NonLazy();
        Container.Bind<ICameraService>().To<CameraService>().AsSingle();

        // MonoBehaviour Services (from prefabs)
        Container.BindInterfacesAndSelfTo<GameService>().FromComponentInNewPrefab(gameElements.gameService).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AudioService>().FromComponentInNewPrefab(gameElements.audioService).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SceneService>().FromComponentInNewPrefab(gameElements.sceneService).AsSingle().NonLazy();

        // MonoBehaviour Services (new GameObjects)
        Container.BindInterfacesAndSelfTo<InputService>().FromNewComponentOnNewGameObject()
            .WithGameObjectName("InputService").AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<BackLogService>().FromNewComponentOnNewGameObject()
            .WithGameObjectName("BackLogService").AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<EffectService>().FromNewComponentOnNewGameObject()
            .WithGameObjectName("EffectService").AsSingle();

        Container.BindInterfacesAndSelfTo<ApiService>().FromNewComponentOnNewGameObject()
            .WithGameObjectName("ApiService").AsSingle().NonLazy();

        // Signals
        Container.DeclareSignal<ScoreUpdateSignal>();

        // UI Canvas
        Container.Bind<Canvas>().FromComponentInNewPrefab(gameElements.globalCanvasPrefab).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<UiOrganizer>().AsSingle();

        // UI Screens (lazy — instantiated on first resolve)
        Container.BindInterfacesAndSelfTo<SplashScreen>().FromComponentInNewPrefab(gameElements.splashScreen).AsSingle();
        Container.BindInterfacesAndSelfTo<HomeScreen>().FromComponentInNewPrefab(gameElements.homeScreen).AsSingle();
        Container.BindInterfacesAndSelfTo<GamePlayScreen>().FromComponentInNewPrefab(gameElements.gamePlayScreen).AsSingle();

        // UI Popups (lazy — instantiated on first resolve)
        Container.BindInterfacesAndSelfTo<CommonPopup>().FromComponentInNewPrefab(gameElements.commonPopup).AsSingle();
        Container.BindInterfacesAndSelfTo<GameFailPopup>().FromComponentInNewPrefab(gameElements.gameFailPopup).AsSingle();
        Container.BindInterfacesAndSelfTo<GameWinPopup>().FromComponentInNewPrefab(gameElements.gameWinPopup).AsSingle();
        Container.BindInterfacesAndSelfTo<GameLosePopup>().FromComponentInNewPrefab(gameElements.gameLosePopup).AsSingle();
        Container.BindInterfacesAndSelfTo<PausePopup>().FromComponentInNewPrefab(gameElements.pausePopup).AsSingle();
        Container.BindInterfacesAndSelfTo<ProfilePopup>().FromComponentInNewPrefab(gameElements.profilePopup).AsSingle();
        Container.BindInterfacesAndSelfTo<SettingsPopup>().FromComponentInNewPrefab(gameElements.settingsPopup).AsSingle();
    }
}