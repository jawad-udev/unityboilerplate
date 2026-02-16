using UnityEngine;

/// <summary>
/// Central configuration ScriptableObject for all environment-specific secrets, tokens, and URLs.
/// Create via: Assets → Create → ScriptableObjects → GameConfig
/// Place the asset at: Resources/Config/GameConfig
///
/// TIP: Add the actual .asset file to .gitignore so secrets never reach version control.
///      Keep a template (GameConfig.template.asset) with placeholder values checked in.
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 0)]
public class GameConfig : ScriptableObject
{
    [Header("API Endpoints")]
    [Tooltip("Base URL for the main REST API (include trailing slash)")]
    public string hostUrl = "http://3.138.70.50:1337/";

    [Tooltip("Base URL for the audio/speech API (include trailing slash)")]
    public string hostAudioUrl = "https://speech-koala.ngrok.app/";

    [Tooltip("CloudFront CDN base URL (include trailing slash)")]
    public string cloudfrontUrl = "https://d1gypmdg0pd8f7.cloudfront.net/";

    [Header("Authentication")]
    [Tooltip("Server-side app token sent as X-App-Token header")]
    public string appToken = "n8r6mTtcAIE6ndABH30DSqfLAtufcM18Dx12eZQv+kYgUEBBrsek5WB2CVKAqkzpg/sEwU8c3Gq";

    [Tooltip("Default X-App-Token added to every request via RequestMessage._defaultHeaders")]
    public string defaultRequestAppToken = "MB7nNL+iePJjTl+21746shJKPsOIu0vshPvCyGjds8t4r31WUDB9QTH4zUkSCxQ3";

    [Header("Game Metadata")]
    [Tooltip("Game version code sent as X-Forwarded-Version header")]
    public string gameVersionCode = "45";

    // ───────────────────────────────────────
    // Singleton accessor (loaded once from Resources)
    // ───────────────────────────────────────
    private static GameConfig _instance;

    public static GameConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameConfig>("Config/GameConfig");
                if (_instance == null)
                    Debug.LogError("GameConfig asset not found at Resources/Config/GameConfig! " +
                                   "Create one via Assets → Create → ScriptableObjects → GameConfig");
            }
            return _instance;
        }
    }
}
