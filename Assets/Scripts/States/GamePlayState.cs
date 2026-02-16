using UnityEngine;
using Zenject;

public class GamePlayState : StateBase
{
	[Inject] private IGameService _gameService;
	[Inject] private IBackLogService _backLogService;
	[Inject] private IUIService _uiService;
	[Inject] private IPlayerService _playerService;

	private float gamePlayDuration;

	public override void OnActivate()
	{
		Debug.Log("Game Play State OnActive");
		_gameService.IsGameActive = true;
		gamePlayDuration = Time.time;

		if (_gameService.GameStatus != GameStatus.PAUSED)
			_backLogService.DisableAndremoveAllScreens();

		_gameService.GameStatus = GameStatus.ONGOING;
		_uiService.ActivateUIScreen(Screens.PLAY);
		_gameService.StartGame();
	}

	public override void OnDeactivate()
	{
		Debug.Log("Game Play State OnDeactivate");
		_playerService.SetTimeSpent(Time.time - gamePlayDuration);
		_gameService.SetGameTime(Time.time - gamePlayDuration);
	}

	public override void OnUpdate()
	{
	}
}