using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UniRx;
using Zenject;

public class ProfilePopup : GameMonoBehaviour
{
    [Inject] private IPlayerService _playerService;
    [Inject] private IBackLogService _backLogService;
    [Inject] private IAudioService _audioService;

    public Button closeButton;
    public TextMeshProUGUI usernameText, levelText, totalScoreText, highScoreText, timeSpentText, gamePlayedText, coinsText;

    private void Awake()
    {
        closeButton.onClick.AsObservable().Subscribe(x => OnClickCloseButton());
    }

    private void Start()
    {
        _playerService.PlayerName.AsObservable().SubscribeToText(usernameText);
        _playerService.TimeSpent.AsObservable().Subscribe(x => timeSpentText.SetText(Mathf.FloorToInt(x / 60) + " mins"));
        _playerService.Level.AsObservable().SubscribeToText(levelText);
        _playerService.HighScore.AsObservable().SubscribeToText(highScoreText);
        _playerService.NumberOfGames.AsObservable().SubscribeToText(gamePlayedText);
        _playerService.Coins.AsObservable().SubscribeToText(coinsText);
        _playerService.TotalScore.AsObservable().SubscribeToText(totalScoreText);
    }

    void OnClickCloseButton()
    {
        _backLogService.CloseLastUI();
        _audioService.PlayUIClick();
    }
}
