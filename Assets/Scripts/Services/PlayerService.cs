using System.IO;
using UnityEngine;
using UniRx;

public class PlayerService : IPlayerService
{
    private const string SAVE_FILE_NAME = "player_data.json";
    private Player _player = null;
    private bool _isDirty = false;

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    // Expose reactive properties through the interface (not the raw model)
    public ReactiveProperty<string> PlayerName => _player.playerName;
    public ReactiveProperty<int> HighScore => _player.highScore;
    public ReactiveProperty<int> TotalScore => _player.totalScore;
    public ReactiveProperty<float> TimeSpent => _player.timeSpent;
    public ReactiveProperty<int> NumberOfGames => _player.numberOfGames;
    public ReactiveProperty<int> Level => _player.level;
    public ReactiveProperty<int> Coins => _player.coins;
    public ReactiveProperty<bool> IsTutorialSeen => _player.isTutorialSeen;

    [SerializeField]
    private string accessToken = string.Empty;
    [SerializeField]
    private string refreshToken = string.Empty;

    private const string AccessTokenKey = "AccessToken_v1";
    private const string RefreshTokenKey = "RefreshToken_v1";

    public string AccessToken
    {
        get => accessToken;
        set
        {
            accessToken = value;
            // Keep TobyTalkClient in sync so static API classes can read the token
            if (Backend.GameClient.Instance != null)
                Backend.GameClient.Instance.AccessToken = value;
        }
    }
    public string RefreshToken { get => refreshToken; set => refreshToken = value; }

    public PlayerService()
    {
        LoadPlayer();

        // Auto-save every 5 seconds if dirty (batched saves instead of saving on every property change)
        Observable.Interval(System.TimeSpan.FromSeconds(5))
            .Subscribe(_ => FlushIfDirty());

        // Save on app quit/pause
        Observable.OnceApplicationQuit()
            .Subscribe(_ => FlushIfDirty());
    }

    private void LoadPlayer()
    {
        try
        {
            if (File.Exists(SaveFilePath))
            {
                string json = File.ReadAllText(SaveFilePath);
                if (!string.IsNullOrEmpty(json))
                {
                    _player = JsonUtility.FromJson<Player>(json);
                    if (_player != null)
                    {
                        Debug.Log("[PlayerService] Loaded player from file.");
                        return;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[PlayerService] Failed to load save file: {ex.Message}");
        }

        // Fallback: try legacy PlayerPrefs migration
        string legacyJson = PlayerPrefs.GetString("PlayerObjString", null);
        if (!string.IsNullOrEmpty(legacyJson) && !legacyJson.Equals("null"))
        {
            _player = JsonUtility.FromJson<Player>(legacyJson);
            Debug.Log("[PlayerService] Migrated player from PlayerPrefs.");
            MarkDirty(); // Will save to file on next flush
            PlayerPrefs.DeleteKey("PlayerObjString"); // Clean up legacy
        }
        else
        {
            _player = new Player("Guest" + Random.Range(10000, 99999), 0, 0, 0f, 0, 1, 5000);
            MarkDirty();
        }
    }

    private void MarkDirty()
    {
        _isDirty = true;
    }

    private void FlushIfDirty()
    {
        if (!_isDirty) return;
        _isDirty = false;

        try
        {
            string json = JsonUtility.ToJson(_player);
            File.WriteAllText(SaveFilePath, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[PlayerService] Failed to save player data: {ex.Message}");
        }
    }

    public void ResetPlayer()
    {
        _player = new Player("Guest" + Random.Range(10000, 99999), 0, 0, 0f, 0, 1, 5000);
        MarkDirty();
        FlushIfDirty();
    }

    #region Public_API

    public void SetPlayerName(string name)
    {
        _player.playerName.Value = name;
        MarkDirty();
    }

    public string GetPlayerName()
    {
        return _player.playerName.Value;
    }

    public void SetHighScore(int score)
    {
        _player.highScore.Value = score;
        MarkDirty();
    }

    public int GetHighScore()
    {
        return _player.highScore.Value;
    }

    public void SetTotalScore(int score)
    {
        _player.totalScore.Value = score;
        MarkDirty();
    }

    public int GetTotalScore()
    {
        return _player.totalScore.Value;
    }

    public void SetNumberOfGames(int count)
    {
        _player.numberOfGames.Value += count;
        MarkDirty();
    }

    public int GetNumberOfGames()
    {
        return _player.numberOfGames.Value;
    }

    public void SetTimeSpent(float time)
    {
        _player.timeSpent.Value += time;
        MarkDirty();
    }

    public float GetTimeSpent()
    {
        return _player.timeSpent.Value;
    }

    public void SetPlayerLevel(int level)
    {
        _player.level.Value = level;
        MarkDirty();
    }

    public void IncrementPlayerLevel(int level)
    {
        _player.level.Value += level;
        MarkDirty();
    }

    public int GetPlayerLevel()
    {
        return _player.level.Value;
    }

    public void SetCoins(int coins)
    {
        _player.coins.Value += coins;
        MarkDirty();
    }

    public int GetPlayerCoins()
    {
        return _player.coins.Value;
    }

    public void SetTutorial(bool isSeen)
    {
        _player.isTutorialSeen.Value = isSeen;
        MarkDirty();
    }

    public bool IsTutorialSeenValue()
    {
        return _player.isTutorialSeen.Value;
    }

    #endregion
}