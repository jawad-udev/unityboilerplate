using UnityEngine;
using System;
using UniRx;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] private ICameraService _cameraService;
    [Inject] private IGameService _gameService;

    #region Variables
    public Camera gameplayCamera;
    CameraShake cameraShake;
    CameraZoom cameraZoom;

    public IReactiveProperty<int> totalLifes { get; private set; }
    #endregion

    private void Awake()
    {
        Init();

        this.PerformActionWithDelay(5f, () =>
        {
            _gameService.SetState<MenuState>();
        });
    }

    public void Init()
    {
        totalLifes = new ReactiveProperty<int>(5);
        _cameraService.AssignPlayerCamera(this);
        gameplayCamera.orthographicSize = _cameraService.ZoomOutLimit;
        cameraShake = gameplayCamera.GetComponent<CameraShake>();
        cameraZoom = gameplayCamera.GetComponent<CameraZoom>();
        cameraZoom.Init(_cameraService);
        ZoomIn();

        totalLifes.Subscribe(x =>
        {
            if (x == 0)
                _gameService.OnGameFinish(false);
        });
    }

    public void ShakeCamera()
    {
        cameraShake.ShakeCamera(0.03f);
    }

    public void ShakeCameraInfinite()
    {
        cameraShake.ShakeCameraInfinite(0.03f);
    }

    public void StopShaking()
    {
        cameraShake.StopShaking();
    }

    public void ZoomIn(Action zoomListener = null)
    {
        cameraZoom.ZoomIn(zoomListener);
    }

    public void ZoomOut(Action zoomListener = null)
    {
        cameraZoom.ZoomOut(zoomListener);
    }
}