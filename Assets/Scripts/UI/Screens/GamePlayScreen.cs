using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;

public class GamePlayScreen : GameMonoBehaviour
{
    [Inject] private IGameService _gameService;
    [Inject] private IUIService _uiService;
    [Inject] private IAudioService _audioService;

    public Button pauseButton, profileButton;

    private void Awake()
    {
        pauseButton.onClick.AsObservable().Subscribe(x => OnClickPauseButton());
        profileButton.onClick.AsObservable().Subscribe(x => OnClickProfileButton());
    }

    public void OnClickProfileButton()
    {
        _uiService.ActivateUIPopups(Popups.PROFILE);
        _audioService.PlayUIClick();
    }

    public void OnClickPauseButton()
    {
        _gameService.SetState<GamePauseState>();
        _audioService.PlayUIClick();
    }
}