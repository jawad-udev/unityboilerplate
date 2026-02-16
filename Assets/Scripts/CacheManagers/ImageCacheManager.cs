using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
/// <summary>
/// Singleton manager responsible for downloading images, caching them to disk (persistentDataPath)
/// and keeping a light in-memory cache.
/// Provides both async/await and Coroutine-based APIs.
/// </summary>


[Serializable]
public class ImageCacheConfig
{
    public string cacheFolder = "ImageCache";
    public int maxMemoryCacheEntries = 50;
    public long fileTTLSeconds = 0; // 0 means never expire
    public bool saveAsPng = false;
}

public class ImageCacheManager : MonoBehaviour
{
    public ImageCacheConfig config;

    private string cacheRootPath;
    private Dictionary<string, Texture2D> memoryCache = new Dictionary<string, Texture2D>();
    private LinkedList<string> memoryCacheOrder = new LinkedList<string>(); // for LRU

    void Awake()
    {
        SetupCacheFolder();
    }

    void SetupCacheFolder()
    {
        string folder = config != null ? config.cacheFolder : "ImageCache";
        cacheRootPath = Path.Combine(Application.persistentDataPath, folder);
        if (!Directory.Exists(cacheRootPath)) Directory.CreateDirectory(cacheRootPath);
    }

    #region Public API (Async)

    public void GetSprite(string url, Action<Sprite> callback)
    {
        if (string.IsNullOrEmpty(url))
        {
            callback?.Invoke(null);
            return;
        }

        // Run async code without making this method async
        StartCoroutine(DownloadSpriteCoroutine(url, callback));
    }

    public void SetSprite(string url, Image image)
    {
        if (string.IsNullOrEmpty(url))
        {
            return;
        }

        // Run async code without making this method async
        StartCoroutine(DownloadSpriteCoroutine(url, sprite =>
        {
            if (sprite != null)
                image.sprite = sprite;
        }));
    }

    private IEnumerator DownloadSpriteCoroutine(string url, Action<Sprite> callback)
    {
        // Wait for async texture load
        var task = GetTextureAsync(url);

        while (!task.IsCompleted)
            yield return null; // wait for next frame until task finishes

        // Handle exceptions if any
        if (task.IsFaulted || task.Result == null)
        {
            Debug.LogWarning($"ImageCache: Failed to download or load sprite for URL: {url}");
            callback?.Invoke(null);
            yield break;
        }

        // Convert to sprite
        var tex = task.Result;
        var sprite = TextureToSprite(tex);
        callback?.Invoke(sprite);
    }

    /// <summary>
    /// Get a Texture2D for the URL. Checks memory cache, then disk cache, then downloads.
    /// Returns null on failure.
    /// </summary>
    public async Task<Texture2D> GetTextureAsync(string url)
    {
        if (string.IsNullOrEmpty(url)) return null;

        // memory cache
        if (memoryCache.TryGetValue(url, out var memTex))
        {
            TouchMemoryEntry(url);
            return memTex;
        }

        // disk cache
        string path = GetCacheFilePath(url);
        if (File.Exists(path))
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (tex.LoadImage(bytes))
                {
                    AddToMemoryCache(url, tex);
                    return tex;
                }
                else
                {
                    // corrupted file - delete
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"ImageCache: Failed to load cached file {path}. Exception: {ex.Message}");
                try { File.Delete(path); } catch { }
            }
        }

        // download
        return await DownloadAndCacheAsync(url);
    }

    /// <summary>
    /// Forces a download and update of cache (useful to refresh stale images).
    /// </summary>
    public async Task<Texture2D> RefreshTextureAsync(string url)
    {
        if (string.IsNullOrEmpty(url)) return null;
        var tex = await DownloadAndCacheAsync(url);
        return tex;
    }

    #endregion

    #region Public API (Coroutines)

    /// <summary>
    /// Coroutine version. Call StartCoroutine(ImageCacheManager.Instance.GetTextureCoroutine(url, onComplete, onError));
    /// </summary>
    public IEnumerator GetTextureCoroutine(string url, Action<Texture2D> onComplete, Action<string> onError = null)
    {
        if (string.IsNullOrEmpty(url)) { onError?.Invoke("url is null or empty"); yield break; }

        // memory
        if (memoryCache.TryGetValue(url, out var memTex))
        {
            TouchMemoryEntry(url);
            onComplete?.Invoke(memTex);
            yield break;
        }

        // disk
        string path = GetCacheFilePath(url);
        if (File.Exists(path))
        {
            byte[] bytes = null;
            try
            {
                bytes = File.ReadAllBytes(path);
                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (tex.LoadImage(bytes))
                {
                    AddToMemoryCache(url, tex);
                    onComplete?.Invoke(tex);
                    yield break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"ImageCache: Failed to load cached file {path}. Exception: {ex.Message}");
                try { File.Delete(path); } catch { }
            }
        }

        // download with UnityWebRequest
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
#else
            if (uwr.isNetworkError || uwr.isHttpError)
#endif
            {
                onError?.Invoke(uwr.error);
                yield break;
            }

            var downloaded = DownloadHandlerTexture.GetContent(uwr);
            if (downloaded != null)
            {
                try
                {
                    byte[] bytes = config != null && !config.saveAsPng ? downloaded.EncodeToJPG() : downloaded.EncodeToPNG();
                    File.WriteAllBytes(path, bytes);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"ImageCache: Failed to write cache file {path}. Exception: {ex.Message}");
                }

                AddToMemoryCache(url, downloaded);
                onComplete?.Invoke(downloaded);
                yield break;
            }
            else
            {
                onError?.Invoke("Downloaded texture is null");
                yield break;
            }
        }
    }

    #endregion

    #region Internal helpers

    private async Task<Texture2D> DownloadAndCacheAsync(string url)
    {
        try
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                var op = uwr.SendWebRequest();
                while (!op.isDone) await Task.Yield();

#if UNITY_2020_1_OR_NEWER
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
#else
                if (uwr.isNetworkError || uwr.isHttpError)
#endif
                {
                    Debug.LogWarning($"ImageCache: Download failed for {url} - {uwr.error}");
                    return null;
                }

                var tex = DownloadHandlerTexture.GetContent(uwr);
                if (tex != null)
                {
                    string path = GetCacheFilePath(url);
                    try
                    {
                        byte[] bytes = config != null && !config.saveAsPng ? tex.EncodeToJPG() : tex.EncodeToPNG();
                        File.WriteAllBytes(path, bytes);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"ImageCache: Failed to write cache file {path}. Exception: {ex.Message}");
                    }

                    AddToMemoryCache(url, tex);
                    return tex;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"ImageCache: Exception downloading {url}: {ex}");
        }
        return null;
    }

    private string GetCacheFilePath(string url)
    {
        string name = GetMd5Hash(url);
        string ext = config != null && !config.saveAsPng ? ".jpg" : ".png";
        return Path.Combine(cacheRootPath, name + ext);
    }

    private static string GetMd5Hash(string input)
    {
        using (var md5 = MD5.Create())
        {
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++) sb.Append(data[i].ToString("x2"));
            return sb.ToString();
        }
    }

    private void AddToMemoryCache(string url, Texture2D tex)
    {
        if (memoryCache.ContainsKey(url))
        {
            memoryCache[url] = tex;
            TouchMemoryEntry(url);
            return;
        }

        memoryCache[url] = tex;
        memoryCacheOrder.AddFirst(url);

        EnforceMemoryLimit();
    }

    private void TouchMemoryEntry(string url)
    {
        // move to front for LRU
        var node = memoryCacheOrder.Find(url);
        if (node != null)
        {
            memoryCacheOrder.Remove(node);
            memoryCacheOrder.AddFirst(node);
        }
    }

    private void EnforceMemoryLimit()
    {
        int max = config != null ? config.maxMemoryCacheEntries : 50;
        if (max <= 0) return;

        while (memoryCacheOrder.Count > max)
        {
            var last = memoryCacheOrder.Last;
            if (last == null) break;
            string key = last.Value;
            memoryCacheOrder.RemoveLast();
            if (memoryCache.TryGetValue(key, out var tex))
            {
                UnityEngine.Object.Destroy(tex);
            }
            memoryCache.Remove(key);
        }
    }

    #endregion

    #region Cache Maintenance

    /// <summary>
    /// Clears all cached files and memory cache.
    /// </summary>
    public void ClearAllCache()
    {
        try
        {
            if (Directory.Exists(cacheRootPath))
            {
                Directory.Delete(cacheRootPath, true);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"ImageCache: Failed to delete cache folder {cacheRootPath}: {ex}");
        }
        finally
        {
            memoryCache.Clear();
            memoryCacheOrder.Clear();
            SetupCacheFolder();
        }
    }

    /// <summary>
    /// Deletes cached files older than TTL (if TTL > 0).
    /// </summary>
    public void ClearExpiredCache()
    {
        if (config == null || config.fileTTLSeconds <= 0) return;

        try
        {
            var files = Directory.GetFiles(cacheRootPath);
            var now = DateTime.UtcNow;
            foreach (var f in files)
            {
                try
                {
                    var info = new FileInfo(f);
                    var ageSec = (now - info.LastWriteTimeUtc).TotalSeconds;
                    if (ageSec > config.fileTTLSeconds) File.Delete(f);
                }
                catch { }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"ImageCache: Failed to clear expired cache: {ex}");
        }
    }

    /// <summary>
    /// Remove a single cached entry (both disk and memory) for a URL.
    /// </summary>
    public void RemoveCached(string url)
    {
        if (string.IsNullOrEmpty(url)) return;
        if (memoryCache.ContainsKey(url))
        {
            var tex = memoryCache[url];
            UnityEngine.Object.Destroy(tex);
            memoryCache.Remove(url);
            memoryCacheOrder.Remove(url);
        }
        string path = GetCacheFilePath(url);
        try { if (File.Exists(path)) File.Delete(path); } catch { }
    }

    /// <summary>
    /// RETURNS true if a file exists on disk for given url (doesn't check memory)
    /// </summary>
    public bool HasCachedFile(string url)
    {
        return File.Exists(GetCacheFilePath(url));
    }

    #endregion

    #region Utility helpers

    public static Sprite TextureToSprite(Texture2D tex, Vector2 pivot = default)
    {
        if (tex == null) return null;
        if (pivot == default) pivot = new Vector2(0.5f, 0.5f);

        int width = tex.width;
        int height = tex.height;

        // If already 1:1, directly convert
        if (width == height)
            return Sprite.Create(tex, new Rect(0, 0, width, height), pivot);

        // Determine square size and start point for cropping
        int size = Mathf.Min(width, height);
        int x = (width - size) / 2;
        int y = (height - size) / 2;

        // Crop from center
        Color[] pixels = tex.GetPixels(x, y, size, size);
        Texture2D croppedTex = new Texture2D(size, size, tex.format, false);
        croppedTex.SetPixels(pixels);
        croppedTex.Apply();

        // Convert cropped texture to sprite
        return Sprite.Create(croppedTex, new Rect(0, 0, size, size), pivot);
    }


    #endregion
}
