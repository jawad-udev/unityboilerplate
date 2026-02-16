using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Zenject;

public class HomeScreen : GameMonoBehaviour
{
    [Inject] private IGameService _gameService;
    [Inject] private IUIService _uiService;
    [Inject] private IAudioService _audioService;
    [Inject] private IPlayerService _playerService;

    public Button playButton;
    public Button profileButton;
    public Button settingsButton;
    public TextMeshProUGUI coinText, usernameText, levelText;

    public Button testButton;

    private void Awake()
    {
        playButton.onClick.AsObservable().Subscribe(x => OnClickPlayButton());
        profileButton.onClick.AsObservable().Subscribe(x => OnClickProfileButton());
        settingsButton.onClick.AsObservable().Subscribe(x => OnClickSettingsButton());
        testButton.onClick.AsObservable().Subscribe(x => Test());
    }

    private void Start()
    {
        _playerService.PlayerName.AsObservable().SubscribeToText(usernameText);
        _playerService.Level.AsObservable().Subscribe(x => levelText.SetText("Level " + x));
        _playerService.Coins.AsObservable().SubscribeToText(coinText);
    }

    private void OnClickProfileButton()
    {
        _uiService.ActivateUIPopups(Popups.PROFILE);
        _audioService.PlayUIClick();
    }

    private void OnClickSettingsButton()
    {
        _uiService.ActivateUIPopups(Popups.SETTINGS);
        _audioService.PlayUIClick();
    }

    private void OnClickPlayButton()
    {
        _uiService.CommonPopup.OpenPopup("Game Mode", "Please select game mode to start game.",
            "Singleplayer", "Multiplayer", OnClickSinglePlayerButton, OnClickMultiplePlayerButton);
        _audioService.PlayUIClick();
    }

    private void OnClickSinglePlayerButton()
    {
        _gameService.GameMode = GameMode.SinglePlayer;
        StartGame();
    }

    private void OnClickMultiplePlayerButton()
    {
        _gameService.GameMode = GameMode.MultiPlayer;
        StartGame();
    }

    private void StartGame()
    {
        _gameService.SetState<GamePlayState>();
    }

    private void Test()
    {
        _playerService.SetCoins(Random.Range(5, 11));
        _playerService.SetHighScore(Random.Range(500, 1100));
        _playerService.SetTotalScore(Random.Range(90, 700000));
        _playerService.SetTimeSpent(Random.Range(500f, 1100f));
        _playerService.SetPlayerLevel(Random.Range(5, 100));
    }
}