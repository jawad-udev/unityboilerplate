public interface IScoreService
{
    int CurrentScore { get; }
    void OnScore(GameplayOwner owner, int scoreIncreaseAmount);
    void CheckHighScore();
    void ResetScore();
}
