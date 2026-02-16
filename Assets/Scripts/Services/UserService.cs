using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Backend; // our generated API classes namespace

/// <summary>
/// Central UserService that calls Backend.*API wrappers and handles tokens, errors, saving profile, auto-refresh.
/// </summary>
public class UserService : MonoBehaviour
{
    public UserInfo userProfile;
    public bool isLoggedIn = false;

    // Local cache for liked media IDs (for instant lookup without API call)
    private HashSet<string> likedMediaIds = new HashSet<string>();
    private bool likedMediaLoaded = false;

    // Local cache for subscribed RMT IDs (for instant lookup without API call)
    private HashSet<int> subscribedRmtIds = new HashSet<int>();
    private bool subscribedRmtsLoaded = false;

    // Store authentication tokens separately (not in userProfile)
    [SerializeField] //Hide in Production
    private string accessToken = string.Empty;
    [SerializeField] //Hide in Production
    private string refreshToken = string.Empty;

    // PlayerPrefs keys for tokens
    private const string AccessTokenKey = "AccessToken_v1";
    private const string RefreshTokenKey = "RefreshToken_v1";

    // Public properties for token access (for API calls)
    public string AccessToken { get => accessToken; set => accessToken = value; }
    public string RefreshToken { get => refreshToken; set => refreshToken = value; }

    // automatic refresh interval (seconds)
    public float refreshInterval = 60f;
    private float timer = 0f;
    private DateTime lastRefreshTime;

    private const string UserProfileKey = "UserProfile_v1";
    private const string EmailCredentialKey = "UserEmail_v1";
    private const string PasswordCredentialKey = "UserPassword_v1";
    private const string RememberMeKey = "RememberMe_v1";



    private void Awake()
    {
        LoadUserProfile();
        lastRefreshTime = DateTime.UtcNow;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= refreshInterval)
        {
            RefreshUserToken();
            timer = 0f;
            lastRefreshTime = DateTime.UtcNow;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            var elapsed = (DateTime.UtcNow - lastRefreshTime).TotalSeconds;

            if (elapsed >= refreshInterval)
            {
                RefreshUserToken();
                timer = 0f;
                lastRefreshTime = DateTime.UtcNow;
            }
            else
            {
                //timer += (float)elapsed;
            }
        }
    }

    void RefreshUserToken()
    {
        if (isLoggedIn)
        {
            // Try refresh silently
            TokenRefresh((tokenResp) =>
            {
                if (tokenResp != null)
                {
                    // update tokens
                    if (!string.IsNullOrEmpty(tokenResp.access)) accessToken = tokenResp.access;
                    if (!string.IsNullOrEmpty(tokenResp.refresh)) refreshToken = tokenResp.refresh;
                    SaveTokens();
                }
            }, (err) =>
            {
                Debug.LogError("[UserService] Token refresh failed: " + err);
            });
        }
    }

    #region Helpers

    private bool IsSuccessStatus(int status)
    {
        return status >= 200 && status < 300;
    }

    private void CheckSession(int status)
    {
        if (status == (int)System.Net.HttpStatusCode.Unauthorized)
        {
            // Unauthorized — clear session and sign out
            SignOut();
            Debug.LogWarning("[UserService] Unauthorized - signing out.");
            // Optionally trigger UI signout flow here.
        }
    }

    private void MapAndStoreTokensFromEntity(object entity)
    {
        if (entity == null) return;

        // Try to find token fields using reflection (case-insensitive)
        Type t = entity.GetType();

        string access = GetStringPropertyIgnoreCase(t, entity, new string[] { "access", "access", "token", "accessToken" });
        string refresh = GetStringPropertyIgnoreCase(t, entity, new string[] { "refresh", "refresh", "refreshToken" });

        if (!string.IsNullOrEmpty(access)) accessToken = access;
        if (!string.IsNullOrEmpty(refresh)) refreshToken = refresh;

        SaveTokens();
    }

    private string GetStringPropertyIgnoreCase(Type t, object obj, string[] names)
    {
        foreach (var n in names)
        {
            var prop = t.GetProperty(n, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                var val = prop.GetValue(obj);
                if (val != null) return val.ToString();
            }

            var field = t.GetField(n, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                var val = field.GetValue(obj);
                if (val != null) return val.ToString();
            }
        }
        return null;
    }

    public void SaveUserProfile()
    {
        if (userProfile == null) userProfile = new UserInfo();
        try
        {
            string json = JsonUtility.ToJson(userProfile);
            PlayerPrefs.SetString(UserProfileKey, json);
            PlayerPrefs.Save();
        }
        catch (Exception ex)
        {
            Debug.LogError("[UserService] SaveUserProfile failed: " + ex);
        }
    }

    private void SaveTokens()
    {
        try
        {
            PlayerPrefs.SetString(AccessTokenKey, accessToken);
            PlayerPrefs.SetString(RefreshTokenKey, refreshToken);
            PlayerPrefs.Save();
        }
        catch (Exception ex)
        {
            Debug.LogError("[UserService] SaveTokens failed: " + ex);
        }
    }

    public void LoadUserProfile()
    {
        if (PlayerPrefs.HasKey(UserProfileKey))
        {
            try
            {
                string json = PlayerPrefs.GetString(UserProfileKey);
                userProfile = JsonUtility.FromJson<UserInfo>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError("[UserService] LoadUserProfile failed: " + ex);
                userProfile = new UserInfo();
            }
        }
        else
        {
            userProfile = new UserInfo();
        }

        LoadTokens();
        isLoggedIn = (userProfile != null && !string.IsNullOrEmpty(accessToken));
    }

    private void LoadTokens()
    {
        try
        {
            accessToken = PlayerPrefs.HasKey(AccessTokenKey) ? PlayerPrefs.GetString(AccessTokenKey) : string.Empty;
            refreshToken = PlayerPrefs.HasKey(RefreshTokenKey) ? PlayerPrefs.GetString(RefreshTokenKey) : string.Empty;
        }
        catch (Exception ex)
        {
            Debug.LogError("[UserService] LoadTokens failed: " + ex);
            accessToken = string.Empty;
            refreshToken = string.Empty;
        }
    }

    public void SignOut()
    {
        userProfile = new UserInfo();
        accessToken = string.Empty;
        refreshToken = string.Empty;
        SaveUserProfile();
        SaveTokens();
        isLoggedIn = false;
        ClearCredentials();
    }

    /// <summary>
    /// Save login credentials for auto sign-in
    /// </summary>
    public void SaveCredentials(string email, string password, bool rememberMe)
    {
        try
        {
            if (rememberMe)
            {
                PlayerPrefs.SetString(EmailCredentialKey, email);
                PlayerPrefs.SetString(PasswordCredentialKey, password);
                PlayerPrefs.SetInt(RememberMeKey, 1);
                PlayerPrefs.Save();
            }
            else
            {
                ClearCredentials();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[UserService] SaveCredentials failed: " + ex);
        }
    }

    /// <summary>
    /// Get saved credentials for auto sign-in
    /// </summary>
    public bool GetSavedCredentials(out string email, out string password)
    {
        email = null;
        password = null;

        try
        {
            if (PlayerPrefs.HasKey(RememberMeKey) && PlayerPrefs.GetInt(RememberMeKey) == 1)
            {
                if (PlayerPrefs.HasKey(EmailCredentialKey) && PlayerPrefs.HasKey(PasswordCredentialKey))
                {
                    email = PlayerPrefs.GetString(EmailCredentialKey);
                    password = PlayerPrefs.GetString(PasswordCredentialKey);
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[UserService] GetSavedCredentials failed: " + ex);
        }

        return false;
    }

    /// <summary>
    /// Clear saved credentials
    /// </summary>
    public void ClearCredentials()
    {
        try
        {
            PlayerPrefs.DeleteKey(EmailCredentialKey);
            PlayerPrefs.DeleteKey(PasswordCredentialKey);
            PlayerPrefs.DeleteKey(RememberMeKey);
            PlayerPrefs.Save();
        }
        catch (Exception ex)
        {
            Debug.LogError("[UserService] ClearCredentials failed: " + ex);
        }
    }

    /// <summary>
    /// Check if credentials are saved for auto sign-in
    /// </summary>
    public bool HasSavedCredentials()
    {
        return PlayerPrefs.HasKey(RememberMeKey) && PlayerPrefs.GetInt(RememberMeKey) == 1 &&
               PlayerPrefs.HasKey(EmailCredentialKey) && PlayerPrefs.HasKey(PasswordCredentialKey);
    }

    #endregion

    #region AUTH Endpoints

    public void TokenRefresh(Action<Backend.TokenRefreshResponse> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            onError?.Invoke("No refresh token available");
            return;
        }

        AuthAPI.TokenRefresh(refreshToken, (response) =>
        {
            if (IsSuccessStatus(response._statusCode) && response._entity != null)
            {
                // update stored tokens
                if (!string.IsNullOrEmpty(response._access)) accessToken = response._access;
                if (!string.IsNullOrEmpty(response._refresh)) refreshToken = response._refresh;
                SaveTokens();

                TokenRefreshResponse data = new TokenRefreshResponse();
                data.access = response._access;
                data.refresh = response._refresh;

                onSuccess?.Invoke(data);
            }
            else
            {
                CheckSession(response._statusCode);
                onError?.Invoke(response._error ?? "Refresh token failed");
            }
        });
    }
    #endregion

    #region Prefetch Cacheing
    // public void PrefetchLikedMedia()
    // {
    //     // Prefetch liked media (all types) to cache
    //     Services.ApiService.GetLikedMedia(
    //         limit: 200,
    //         offset: 0,
    //         mediaTypeCsv: null,
    //         responseListener: (response) =>
    //         {
    //             if (response._status && response._entity?.results != null)
    //             {
    //                 likedMediaIds.Clear();
    //                 foreach (var media in response._entity.results)
    //                 {
    //                     if (!string.IsNullOrEmpty(media.id))
    //                         likedMediaIds.Add(media.id);
    //                 }
    //                 likedMediaLoaded = true;
    //                 Debug.Log($"[UserService] Liked media cached successfully. Count: {likedMediaIds.Count}");
    //             }
    //         }
    //     );
    // }

    // /// <summary>
    // /// Check if a media is liked (synchronous, instant lookup from local cache)
    // /// </summary>
    // public bool IsMediaLiked(string mediaId)
    // {
    //     if (string.IsNullOrEmpty(mediaId))
    //         return false;
    //     return likedMediaIds.Contains(mediaId);
    // }

    // /// <summary>
    // /// Check if liked media data has been loaded
    // /// </summary>
    // public bool IsLikedMediaLoaded => likedMediaLoaded;

    /// <summary>
    /// Update local liked state (call after like toggle)
    /// </summary>
    // public void SetMediaLiked(string mediaId, bool isLiked)
    // {
    //     if (string.IsNullOrEmpty(mediaId))
    //         return;

    //     if (isLiked)
    //         likedMediaIds.Add(mediaId);
    //     else
    //         likedMediaIds.Remove(mediaId);
    // }
    // public void PrefetchSubscribedRmts()
    // {
    //     // Prefetch subscribed RMTs to cache
    //     Services.ApiService.GetMySubscribedRmts(
    //         limit: 100,
    //         offset: 0,
    //         responseListener: (response) =>
    //         {
    //             if (response._status && response._entity?.results != null)
    //             {
    //                 subscribedRmtIds.Clear();
    //                 foreach (var rmt in response._entity.results)
    //                 {
    //                     subscribedRmtIds.Add(rmt.id);
    //                 }
    //                 subscribedRmtsLoaded = true;
    //                 Debug.Log($"[UserService] Subscribed RMTs cached successfully. Count: {subscribedRmtIds.Count}");
    //             }
    //         }
    //     );
    // }

    /// <summary>
    /// Check if subscribed to an RMT (synchronous, instant lookup from local cache)
    /// </summary>
    // public bool IsSubscribedToRmt(int rmtId)
    // {
    //     return subscribedRmtIds.Contains(rmtId);
    // }

    // /// <summary>
    // /// Check if subscribed RMT data has been loaded
    // /// </summary>
    // public bool IsSubscribedRmtsLoaded => subscribedRmtsLoaded;

    // /// <summary>
    // /// Update local subscribed state (call after subscribe/unsubscribe toggle)
    // /// </summary>
    // public void SetRmtSubscribed(int rmtId, bool isSubscribed)
    // {
    //     if (isSubscribed)
    //         subscribedRmtIds.Add(rmtId);
    //     else
    //         subscribedRmtIds.Remove(rmtId);
    // }

    // public void PrefetchCacheData()
    // {
    //     PrefetchLikedMedia();
    //     PrefetchSubscribedRmts();
    // }
    #endregion
}