using UnityEngine;
using Zenject;

public class MenuState : StateBase
{
	[Inject] private IBackLogService _backLogService;
	[Inject] private IUIService _uiService;
	[Inject] private IGameService _gameService;
	[Inject] private ICameraService _cameraService;
	[Inject] private IAudioService _audioService;
	[Inject] private ISceneService _sceneService;

	public override void OnActivate()
	{
		Debug.Log("Menu State OnActive");
		_backLogService.DisableAndremoveAllScreens();
		_uiService.ActivateUIScreen(Screens.HOME);
		_gameService.GameStatus = GameStatus.TOSTART;
		_cameraService.ResetCameraSize();
		_audioService.PlayGameMusic();
		_sceneService.LoadMainScene();
	}

	public override void OnDeactivate()
	{
		Debug.Log("Menu State OnDeactivate");
	}

	public override void OnUpdate()
	{
	}
}