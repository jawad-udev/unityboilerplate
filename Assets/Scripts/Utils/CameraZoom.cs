using UnityEngine;
using System;
using Zenject;

public class CameraZoom : MonoBehaviour
{
    [Inject] private ICameraService _cameraService;

    Action zoomCallback;
    public Camera camera;

    public void ZoomIn(Action zoomListener = null)
    {
        zoomCallback = zoomListener;

        if (camera.orthographicSize > _cameraService.ZoomInLimit)
        {
            StartTween(_cameraService.ZoomOutLimit, _cameraService.ZoomInLimit);
        }
    }

    public void ZoomOut(Action zoomListener = null)
    {
        zoomCallback = zoomListener;

        if (camera.orthographicSize < _cameraService.ZoomOutLimit)
            StartTween(_cameraService.ZoomInLimit, _cameraService.ZoomOutLimit);
    }

    void StartTween(float initialValue, float finalValue)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", initialValue, "to", finalValue, "time", _cameraService.ZoomSpeed,
            "easetype", _cameraService.EaseType, "onupdatetarget", gameObject, "onupdate", "OnUpdateValue", "oncomplete", "OnTweenComplete"));
    }

    void OnUpdateValue(float newValue)
    {
        camera.orthographicSize = newValue;
    }

    void OnTweenComplete()
    {
        zoomCallback?.Invoke();
    }
}
