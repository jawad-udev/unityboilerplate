using UnityEngine;
using System;
using DG.Tweening;

public class CameraZoom : MonoBehaviour
{
    private ICameraService _cameraService;

    private Tween _currentTween;
    public Camera mainCamera;

    public void Init(ICameraService cameraService)
    {
        _cameraService = cameraService;
    }

    public void ZoomIn(Action zoomListener = null)
    {
        if (mainCamera.orthographicSize > _cameraService.ZoomInLimit)
        {
            StartTween(_cameraService.ZoomInLimit, zoomListener);
        }
    }

    public void ZoomOut(Action zoomListener = null)
    {
        if (mainCamera.orthographicSize < _cameraService.ZoomOutLimit)
        {
            StartTween(_cameraService.ZoomOutLimit, zoomListener);
        }
    }

    private void StartTween(float targetValue, Action onComplete)
    {
        _currentTween?.Kill();

        _currentTween = DOTween.To(
                () => mainCamera.orthographicSize,
                x => mainCamera.orthographicSize = x,
                targetValue,
                _cameraService.ZoomSpeed
            )
            .SetEase(_cameraService.EaseType)
            .OnComplete(() => onComplete?.Invoke());
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
    }
}
