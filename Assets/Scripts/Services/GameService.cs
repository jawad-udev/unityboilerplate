using UnityEngine;
using System;
using System.Collections.Generic;
using Zenject;

public class GameService : MonoBehaviour, IGameService
{
	[Inject] private ISceneService _sceneService;
	[Inject] private IAudioService _audioService;
	[Inject] private ICameraService _cameraService;

	public GameMode GameMode { get; set; }
	public GameStatus GameStatus { get; set; }
	public bool IsGameActive { get; set; }
	public GameManager CurrentGameManager { get; set; }
	public float GameTime => gameTime;

	// Legacy fields kept for inspector compatibility
	public GameMode gameMode { get => GameMode; set => GameMode = value; }
	public GameStatus gameStatus { get => GameStatus; set => GameStatus = value; }
	public bool isGameActive { get => IsGameActive; set => IsGameActive = value; }
	public GameManager gameManager { get => CurrentGameManager; set => CurrentGameManager = value; }
	public ColorService colorService;

	private float gameTime;
	private StateBase currentState;
	private Dictionary<Type, StateBase> _states;

	public StateBase State => currentState;

	public void Start()
	{
		// Pre-register all states in a dictionary for O(1) lookup instead of GetComponentInChildren
		_states = new Dictionary<Type, StateBase>();
		foreach (var state in GetComponentsInChildren<StateBase>(true))
		{
			_states[state.GetType()] = state;
		}

		IsGameActive = false;
		SetState<SplashState>();
	}

	public void SetState(Type newStateType)
	{
		currentState?.OnDeactivate();

		if (_states.TryGetValue(newStateType, out StateBase nextState))
		{
			currentState = nextState;
			currentState.OnActivate();
		}
		else
		{
			Debug.LogError($"[GameService] State not found: {newStateType.Name}");
		}
	}

	public void SetState<T>() where T : StateBase
	{
		SetState(typeof(T));
	}

	void Update()
	{
		currentState?.OnUpdate();
	}

	#region GamePlay Managers

	public void StartGame()
	{
		_sceneService.LoadGameScene();
		ResetGameTime();
		_audioService.RestartGameMusic();
	}

	public GameManager SpawnGamePlay(GameManager _gameplayManager)
	{
		GameManager gameplayManager = Instantiate(_gameplayManager);
		gameplayManager.gameObject.name = "GameManager";
		_cameraService.AssignPlayerCamera(gameplayManager);
		gameplayManager.Init();
		return gameplayManager;
	}

	public GameManager GetPlayerManager(GameplayOwner gameplayOwner)
	{
		return CurrentGameManager;
	}

	public void DestroyGameplayManager()
	{
		if (CurrentGameManager != null && CurrentGameManager.gameObject != null)
			DestroyImmediate(CurrentGameManager.gameObject);
	}

	// Keep old name as alias for backwards compatibility with existing prefab references
	public void DestryoGameplayManager() => DestroyGameplayManager();

	public void SetGameTime(float time)
	{
		gameTime += time;
	}

	public float GetGameTime()
	{
		return gameTime;
	}

	private void ResetGameTime()
	{
		gameTime = 0;
	}

	#endregion

	#region Game Finisher

	public void OnGameFinish(bool isWin)
	{
		if (isWin)
		{
			GameStatus = GameStatus.WON;

			_cameraService.ZoomOut(CurrentGameManager, () =>
			{
				Extensions.PerformActionWithDelay(this, 2f, SetState<GameOverState>);
			});
		}
		else
		{
			GameStatus = GameStatus.LOST;
			SetState<GameOverState>();
		}
	}

	#endregion
}