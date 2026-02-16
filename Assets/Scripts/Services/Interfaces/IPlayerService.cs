using UniRx;

public interface IPlayerService
{
    ReactiveProperty<string> PlayerName { get; }
    ReactiveProperty<int> HighScore { get; }
    ReactiveProperty<int> TotalScore { get; }
    ReactiveProperty<float> TimeSpent { get; }
    ReactiveProperty<int> NumberOfGames { get; }
    ReactiveProperty<int> Level { get; }
    ReactiveProperty<int> Coins { get; }
    ReactiveProperty<bool> IsTutorialSeen { get; }

    void SetPlayerName(string name);
    string GetPlayerName();
    void SetHighScore(int score);
    int GetHighScore();
    void SetTotalScore(int score);
    int GetTotalScore();
    void SetNumberOfGames(int count);
    int GetNumberOfGames();
    void SetTimeSpent(float time);
    float GetTimeSpent();
    void SetPlayerLevel(int level);
    void IncrementPlayerLevel(int level);
    int GetPlayerLevel();
    void SetCoins(int coins);
    int GetPlayerCoins();
    void SetTutorial(bool isSeen);
    bool IsTutorialSeenValue();
    void ResetPlayer();
}
