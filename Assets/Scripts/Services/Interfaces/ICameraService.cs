using System;
using UnityEngine;
using DG.Tweening;

public interface ICameraService
{
    float ZoomOutLimit { get; }
    float ZoomInLimit { get; }
    float ZoomSpeed { get; }
    Ease EaseType { get; }

    Camera AssignPlayerCamera(GameplayOwner owner);
    Camera AssignPlayerCamera(GameManager gameplayManager);
    void ShakeCamera(GameManager gameplay);
    void ShakeCamera(GameplayOwner gameplayOwner);
    void ZoomIn(GameManager gameplay, Action zoomListener = null);
    void ZoomOut(GameManager gameplay, Action zoomListener = null);
    Vector3 GetCameraTopPosition(Camera cam);
    Vector3 GetCameraTopPosition(GameplayOwner gameplayOwner);
    float GetCameraHeight(Camera cam);
    bool IsInCamView(GameplayOwner gameplayOwner, Vector3 pos);
    void ResetCameraSize();
}
