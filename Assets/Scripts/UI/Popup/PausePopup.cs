using UnityEngine.UI;
using UnityEngine;
using UniRx;
using Zenject;

public class PausePopup : GameMonoBehaviour
{
    [Inject] private IGameService _gameService;
    [Inject] private IAudioService _audioService;

    public Button resumeButton, restartButton, exitButton;

    void Awake()
    {
        resumeButton.onClick.AsObservable().Subscribe(x => OnClickResumeButton());
        restartButton.onClick.AsObservable().Subscribe(x => OnClickRestartButton());
        exitButton.onClick.AsObservable().Subscribe(x => OnClickExitButton());
    }

    void OnClickResumeButton()
    {
        _gameService.SetState<GamePlayState>();
        _audioService.PlayUIClick();
    }

    void OnClickRestartButton()
    {
        _gameService.DestroyGameplayManager();
        _gameService.GameStatus = GameStatus.TOSTART;
        _gameService.SetState<GamePlayState>();
        _audioService.PlayUIClick();
    }

    void OnClickExitButton()
    {
        _gameService.SetState<MenuState>();
        _audioService.PlayUIClick();
    }
}
