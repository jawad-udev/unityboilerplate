using UnityEngine;
using Zenject;

public class GameOverState : StateBase
{
	[Inject] private IGameService _gameService;
	[Inject] private IUIService _uiService;
	[Inject] private IAudioService _audioService;
	[Inject] private IBackLogService _backLogService;
	[Inject] private IPlayerService _playerService;
	[Inject] private IScoreService _scoreService;

	public override void OnActivate()
	{
		Debug.Log("Game Over State OnActive");

		if (!_gameService.IsGameActive)
			return;

		_gameService.IsGameActive = false;

		if (_gameService.GameStatus == GameStatus.WON)
		{
			_uiService.ActivateUIPopups(Popups.WIN);
			_audioService.PlayWinSound();
		}
		else if (_gameService.GameStatus == GameStatus.LOST)
		{
			if (_gameService.GameMode == GameMode.SinglePlayer)
			{
				_uiService.ActivateUIPopups(Popups.FAIL);
				_audioService.PlayLoseSound();
			}
			else
			{
				_uiService.ActivateUIPopups(Popups.LOSE);
				_audioService.PlayLoseSound();
			}
		}

		_backLogService.RemoveLastScreens(1);
		_playerService.SetHighScore(_scoreService.CurrentScore);
		_playerService.SetNumberOfGames(1);
	}

	public override void OnDeactivate()
	{
		Debug.Log("Game Over State OnDeactivate");
		_gameService.GameStatus = GameStatus.TOSTART;
		_gameService.DestroyGameplayManager();
	}

	public override void OnUpdate()
	{
	}
}