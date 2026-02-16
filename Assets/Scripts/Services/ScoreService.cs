using UnityEngine;
using Zenject;

public class ScoreService : IScoreService
{
    [Inject] private IPlayerService _playerService;

    public int CurrentScore => currentScore;
    public int currentScore = 0;

    public void OnScore(GameplayOwner owner, int scoreIncreaseAmount)
    {
        if (owner == GameplayOwner.Player1)
        {
            currentScore += scoreIncreaseAmount;
            CheckHighScore();
            _playerService.SetTotalScore(_playerService.GetTotalScore() + scoreIncreaseAmount);
        }
        else if (owner == GameplayOwner.Player2)
        {
            // Player-2 score can be handled here
        }
    }

    public void CheckHighScore()
    {
        if (_playerService.GetHighScore() < currentScore)
        {
            _playerService.SetHighScore(currentScore);
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
    }
}