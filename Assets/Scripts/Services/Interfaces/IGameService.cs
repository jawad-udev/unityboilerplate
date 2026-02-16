public interface IGameService
{
    GameMode GameMode { get; set; }
    GameStatus GameStatus { get; set; }
    bool IsGameActive { get; set; }
    GameManager CurrentGameManager { get; set; }
    float GameTime { get; }

    void SetState<T>() where T : StateBase;
    void SetState(System.Type newStateType);
    void StartGame();
    GameManager SpawnGamePlay(GameManager gameplayManager);
    GameManager GetPlayerManager(GameplayOwner gameplayOwner);
    void DestroyGameplayManager();
    void SetGameTime(float time);
    float GetGameTime();
    void OnGameFinish(bool isWin);
}
