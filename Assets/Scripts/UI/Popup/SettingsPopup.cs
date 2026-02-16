using UnityEngine.UI;
using UnityEngine;
using UniRx;
using Zenject;

public class SettingsPopup : GameMonoBehaviour
{
    [Inject] private IAudioService _audioService;
    [Inject] private IBackLogService _backLogService;
    [Inject] private IPlayerService _playerService;

    public Button musicButton, soundButton, vibrationButton, closeButton, clearData, showDebugButton;

    void Awake()
    {
        closeButton.onClick.AsObservable().Subscribe(x => OnClickCloseButton());
        clearData.onClick.AsObservable().Subscribe(x => OnClickClearDataButton());
        showDebugButton.onClick.AsObservable().Subscribe(x => OnClickShowDebugButton());
    }

    void OnClickMusicButton()
    {
        _audioService.PlayUIClick();
    }

    void OnClickSoundButton()
    {
        _audioService.PlayUIClick();
    }

    void OnClickVibrationButton()
    {
        _audioService.PlayUIClick();
    }

    void OnClickCloseButton()
    {
        _backLogService.CloseLastUI();
        _audioService.PlayUIClick();
    }

    void OnClickClearDataButton()
    {
        _playerService.ResetPlayer();
        clearData.interactable = false;
        _audioService.PlayUIClick();
    }

    void OnClickShowDebugButton()
    {
        _audioService.PlayUIClick();
    }
}
