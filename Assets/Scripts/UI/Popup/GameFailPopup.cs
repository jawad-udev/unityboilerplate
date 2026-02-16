using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UniRx;
using Zenject;

public class GameFailPopup : GameMonoBehaviour
{
    [Inject] private IGameService _gameService;
    [Inject] private IAudioService _audioService;
    [Inject] private IPlayerService _playerService;

    public Button restartButton, homeButton;
    public TextMeshProUGUI coinText;

    void Awake()
    {
        restartButton.onClick.AsObservable().Subscribe(x => OnClickRestartButton());
        homeButton.onClick.AsObservable().Subscribe(x => OnClickHomeButton());
    }

    private void OnEnable()
    {
        DeductCoin();
    }

    void OnClickRestartButton()
    {
        _gameService.GameStatus = GameStatus.TOSTART;
        _gameService.DestroyGameplayManager();
        _gameService.SetState<GamePlayState>();
        _audioService.PlayUIClick();
        this.Hide();
    }

    void OnClickHomeButton()
    {
        _gameService.SetState<MenuState>();
        _audioService.PlayUIClick();
        this.Hide();
    }

    void DeductCoin()
    {
        int coinsToDeduct = Random.Range(-500, -100);
        coinText.SetText(coinsToDeduct.ToString());
        _playerService.SetCoins(coinsToDeduct);
    }
}
