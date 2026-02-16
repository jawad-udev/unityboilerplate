public interface IAudioService
{
    bool IsSoundEnabled { get; set; }
    bool IsMusicEnabled { get; set; }

    void PlaySound(UnityEngine.AudioClip clip);
    void PlayUIClick();
    void PlayWinSound();
    void PlayLoseSound();
    void PlaySplashScreenSound();
    void PlayPopUpOpenSound();
    void PlayPopUpCloseSound();
    void PlayShapePlaceSound();
    void PlayShapeMoveSound();
    void PlayExplosionSound();
    void PlayBlockSpawnSound();
    void PlayTimeCountDownSound();
    void PlayShapeRotateSound();
    void SetSoundFxVolume(float value);

    void EnableGameMusic(bool enable);
    void PlayGameMusic();
    void RestartGameMusic();
    void StopGameMusic();
    void SetSoundMusicVolume(float value);
}
