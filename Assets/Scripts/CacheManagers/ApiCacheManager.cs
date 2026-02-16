using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum CacheType
{
    Persistent,
    Session
}

public static class ApiCacheManager
{
    // Default TTL in seconds (change this to your desired default 'x')
    public static int DefaultTTLSeconds = 120000;

    private static readonly object sessionLock = new object();
    private static readonly object fileLock = new object();

    // Session cache stores metadata so we can expire entries per key
    private class SessionCacheEntry
    {
        public string Data;
        public long SavedAtUtcSeconds;
        public int TtlSeconds;
    }

    private static Dictionary<string, SessionCacheEntry> sessionCache = new Dictionary<string, SessionCacheEntry>();

    // Track original URLs for prefix-based invalidation
    private static Dictionary<string, string> keyToOriginalUrl = new Dictionary<string, string>();

    private static string PersistentPath => Path.Combine(Application.persistentDataPath, "api_cache");

    [Serializable]
    private class PersistentCacheWrapper
    {
        public long savedAtUtcSeconds;
        public int ttlSeconds;
        public string data;
    }

    static ApiCacheManager()
    {
        try
        {
            if (!Directory.Exists(PersistentPath))
                Directory.CreateDirectory(PersistentPath);
        }
        catch (Exception ex)
        {
            Debug.LogError($"ApiCacheManager: Failed ensuring persistent path: {ex}");
        }
    }

    /// <summary>
    /// Save a cached response. ttlSeconds overrides the default TTL when > 0.
    /// </summary>
    public static void Save(string key, string json, CacheType type, int ttlSeconds = -1)
    {
        string originalKey = key;
        key = GetSafeFileName(key);
        if (ttlSeconds <= 0)
            ttlSeconds = DefaultTTLSeconds;

        if (type == CacheType.Persistent)
        {
            try
            {
                // Ensure cache directory exists
                if (!Directory.Exists(PersistentPath))
                    Directory.CreateDirectory(PersistentPath);

                var wrapper = new PersistentCacheWrapper
                {
                    savedAtUtcSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    ttlSeconds = ttlSeconds,
                    data = json
                };

                string filePath = Path.Combine(PersistentPath, key + ".json");

                lock (fileLock)
                {
                    File.WriteAllText(filePath, JsonUtility.ToJson(wrapper));
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"ApiCacheManager: Failed to save persistent cache for '{key}': {ex}");
            }
        }
        else
        {
            var entry = new SessionCacheEntry()
            {
                Data = json,
                SavedAtUtcSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                TtlSeconds = ttlSeconds
            };

            lock (sessionLock)
            {
                sessionCache[key] = entry;
                keyToOriginalUrl[key] = originalKey;
                Debug.Log("Total links cached : " + sessionCache.Count);
            }
        }
    }

    /// <summary>
    /// Try to load cached entry. Returns false if not found or expired.
    /// </summary>
    public static bool TryLoad(string key, CacheType type, out string json)
    {
        key = GetSafeFileName(key);
        json = null;

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (type == CacheType.Persistent)
        {
            string file = Path.Combine(PersistentPath, key + ".json");
            try
            {
                lock (fileLock)
                {
                    if (!File.Exists(file))
                        return false;

                    string text = File.ReadAllText(file);
                    if (string.IsNullOrEmpty(text))
                    {
                        // Corrupt/empty file - delete and return false
                        TryDeleteFile(file);
                        return false;
                    }

                    PersistentCacheWrapper wrapper;
                    try
                    {
                        wrapper = JsonUtility.FromJson<PersistentCacheWrapper>(text);
                    }
                    catch
                    {
                        // Bad JSON - delete and return false
                        TryDeleteFile(file);
                        return false;
                    }

                    if (wrapper == null || string.IsNullOrEmpty(wrapper.data))
                    {
                        TryDeleteFile(file);
                        return false;
                    }

                    if (IsExpired(wrapper.savedAtUtcSeconds, wrapper.ttlSeconds, now))
                    {
                        TryDeleteFile(file);
                        return false;
                    }

                    json = wrapper.data;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"ApiCacheManager: Error reading persistent cache '{key}': {ex}");
                return false;
            }
        }
        else
        {
            lock (sessionLock)
            {
                if (!sessionCache.ContainsKey(key))
                    return false;

                var entry = sessionCache[key];
                if (IsExpired(entry.SavedAtUtcSeconds, entry.TtlSeconds, now))
                {
                    // expired — remove and return false
                    sessionCache.Remove(key);
                    Debug.Log($"ApiCacheManager: Session cache expired and removed for key '{key}'");
                    return false;
                }

                json = entry.Data;
                return true;
            }
        }
    }

    private static bool IsExpired(long savedAtUtcSeconds, int ttlSeconds, long nowUtcSeconds)
    {
        // ttlSeconds <= 0 means "never expire" — but by our API we always set >0; keep check for safety
        if (ttlSeconds <= 0) return false;
        return (nowUtcSeconds - savedAtUtcSeconds) >= ttlSeconds;
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"ApiCacheManager: Failed to delete file '{path}': {ex}");
        }
    }

    public static void ClearSession()
    {
        lock (sessionLock)
        {
            sessionCache.Clear();
        }
    }

    /// <summary>
    /// Remove a specific key from cache (session or persistent).
    /// </summary>
    public static void Remove(string key, CacheType type)
    {
        key = GetSafeFileName(key);
        if (type == CacheType.Persistent)
        {
            string file = Path.Combine(PersistentPath, key + ".json");
            lock (fileLock)
            {
                TryDeleteFile(file);
            }
        }
        else
        {
            lock (sessionLock)
            {
                if (sessionCache.ContainsKey(key))
                    sessionCache.Remove(key);
                if (keyToOriginalUrl.ContainsKey(key))
                    keyToOriginalUrl.Remove(key);
            }
        }
    }

    /// <summary>
    /// Remove all session cache entries where the original URL contains the specified prefix.
    /// </summary>
    public static void RemoveByUrlPrefix(string urlPrefix)
    {
        lock (sessionLock)
        {
            var keysToRemove = new List<string>();
            foreach (var kvp in keyToOriginalUrl)
            {
                if (kvp.Value.Contains(urlPrefix))
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                sessionCache.Remove(key);
                keyToOriginalUrl.Remove(key);
                Debug.Log($"[ApiCacheManager] Removed cache with prefix '{urlPrefix}'");
            }

            if (keysToRemove.Count > 0)
                Debug.Log($"[ApiCacheManager] Invalidated {keysToRemove.Count} cache entries matching '{urlPrefix}'");
        }
    }

    /// <summary>
    /// Clear all persistent cache files
    /// </summary>
    public static void ClearPersistent()
    {
        try
        {
            lock (fileLock)
            {
                if (Directory.Exists(PersistentPath))
                {
                    var files = Directory.GetFiles(PersistentPath, "*.json");
                    foreach (var f in files)
                    {
                        TryDeleteFile(f);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"ApiCacheManager: Failed to clear persistent cache: {ex}");
        }
    }

    private static string GetSafeFileName(string key)
    {
        using (var sha1 = System.Security.Cryptography.SHA1.Create())
        {
            byte[] hash = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
