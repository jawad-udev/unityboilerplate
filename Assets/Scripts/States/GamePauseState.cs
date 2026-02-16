using UnityEngine;
using Zenject;

public class GamePauseState : StateBase
{
	[Inject] private IUIService _uiService;
	[Inject] private IGameService _gameService;

	public override void OnActivate()
	{
		Debug.Log("Game Pause State OnActive");
		_uiService.ActivateUIPopups(Popups.PAUSE);
		_gameService.GameStatus = GameStatus.PAUSED;
	}

	public override void OnDeactivate()
	{
		Debug.Log("Game Pause State OnDeactivate");
	}

	public override void OnUpdate()
	{
	}
}