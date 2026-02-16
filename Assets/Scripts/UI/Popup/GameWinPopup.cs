using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UniRx;
using Zenject;

public class GameWinPopup : GameMonoBehaviour
{
    [Inject] private IGameService _gameService;
    [Inject] private IAudioService _audioService;
    [Inject] private IPlayerService _playerService;

    public Button restartButton, homeButton;
    public TextMeshProUGUI coinText, scoreText, timeText, ShapesPlacedText, titleText;

    void Awake()
    {
        restartButton.onClick.AsObservable().Subscribe(x => OnClickRestartButton());
        homeButton.onClick.AsObservable().Subscribe(x => OnClickHomeButton());
    }

    private void OnEnable()
    {
        GiftCoin();
        UpdateGameTime();
        UpdateGameScore();
        UpdateGameShapes();
        SetTitleText();
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

    void GiftCoin()
    {
        int coins = Random.Range(100, 500);
        coinText.SetText("+" + coins.ToString());
        _playerService.SetCoins(coins);
    }

    void UpdateGameTime()
    {
        float time = _gameService.GetGameTime();
        timeText.SetText(Mathf.FloorToInt(time).ToString() + " Secs");
    }

    void UpdateGameShapes()
    {
    }

    void UpdateGameScore()
    {
        int score = 0;
        scoreText.SetText(score.ToString());
    }

    void SetTitleText()
    {
        if (_gameService.GameMode == GameMode.SinglePlayer)
        {
            titleText.SetText("TOWER COMPLETED");
        }
        else if (_gameService.GameMode == GameMode.MultiPlayer)
        {
            titleText.SetText("YOU WON");
        }
    }
}
