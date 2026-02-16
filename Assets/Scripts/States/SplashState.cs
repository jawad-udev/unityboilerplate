using UnityEngine;
using Zenject;

public class SplashState : StateBase
{
	[Inject] private IUIService _uiService;
	[Inject] private IGameService _gameService;

	public override void OnActivate()
	{
		Debug.Log("Splash State OnActive");
		_uiService.ActivateUIScreen(Screens.SPLASH);
		_gameService.GameStatus = GameStatus.TOSTART;
	}

	public override void OnDeactivate()
	{
		Debug.Log("Splash State OnDeactivate");
	}

	public override void OnUpdate()
	{
	}
}