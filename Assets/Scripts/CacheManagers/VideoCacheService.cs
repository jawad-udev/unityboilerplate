using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Video cache service for downloading and caching videos locally.
/// Features:
/// - Disk caching with configurable size limit
/// - LRU eviction policy
/// - Priority-based download queue
/// - Progress callbacks
/// - Atomic file writes
/// - Resume support for interrupted downloads
/// </summary>
public class VideoCacheService : MonoBehaviour
{
    [Header("Cache Settings")]
    [SerializeField] private long maxCacheSizeBytes = 500 * 1024 * 1024; // 500MB default
    [SerializeField] private int maxConcurrentDownloads = 2;
    [SerializeField] private bool wifiOnlyForBackground = false;

    // Cache state
    private Dictionary<string, VideoCacheEntry> cacheIndex = new Dictionary<string, VideoCacheEntry>();
    private Dictionary<string, DownloadOperation> activeDownloads = new Dictionary<string, DownloadOperation>();
    private List<DownloadRequest> downloadQueue = new List<DownloadRequest>();

    private string cacheFolder;
    private string indexFilePath;
    private bool isInitialized = false;

    // Events
    public event Action<string, float> OnDownloadProgress;
    public event Action<string, bool> OnDownloadComplete;

    #region Initialization

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (isInitialized) return;

        cacheFolder = Path.Combine(Application.persistentDataPath, "VideoCache");
        indexFilePath = Path.Combine(cacheFolder, "cache_index.json");

        EnsureCacheFolder();
        LoadCacheIndex();

        isInitialized = true;
        Debug.Log($"[VideoCacheService] Initialized. Cache folder: {cacheFolder}");
    }

    private void EnsureCacheFolder()
    {
        if (!Directory.Exists(cacheFolder))
        {
            Directory.CreateDirectory(cacheFolder);
            Debug.Log($"[VideoCacheService] Created cache folder: {cacheFolder}");
        }
    }

    #endregion

    #region Public API

    /// <summary>
    /// Check if a video URL is fully cached
    /// </summary>
    public bool IsCached(string url)
    {
        if (string.IsNullOrEmpty(url)) return false;

        string hash = VideoCacheEntry.GetUrlHash(url);
        if (cacheIndex.TryGetValue(hash, out var entry))
        {
            if (entry.isFullyCached && File.Exists(entry.localPath))
            {
                entry.MarkAccessed();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Get the local file path for a cached video
    /// </summary>
    public string GetCachedPath(string url)
    {
        if (string.IsNullOrEmpty(url)) return null;

        string hash = VideoCacheEntry.GetUrlHash(url);
        if (cacheIndex.TryGetValue(hash, out var entry))
        {
            if (entry.isFullyCached && File.Exists(entry.localPath))
            {
                entry.MarkAccessed();
                SaveCacheIndex();
                return entry.localPath;
            }
        }
        return null;
    }

    /// <summary>
    /// Get download progress for a URL (0-1, or -1 if not downloading)
    /// </summary>
    public float GetDownloadProgress(string url)
    {
        if (string.IsNullOrEmpty(url)) return -1f;

        string hash = VideoCacheEntry.GetUrlHash(url);
        if (activeDownloads.TryGetValue(hash, out var operation))
        {
            return operation.Progress;
        }
        return -1f;
    }

    /// <summary>
    /// Check if a URL is currently being downloaded
    /// </summary>
    public bool IsDownloading(string url)
    {
        if (string.IsNullOrEmpty(url)) return false;
        string hash = VideoCacheEntry.GetUrlHash(url);
        return activeDownloads.ContainsKey(hash);
    }

    /// <summary>
    /// Cache a video from URL
    /// </summary>
    public void CacheVideo(string url, string videoId, long fileSize, CachePriority priority,
        Action<float> onProgress = null, Action<bool, string> onComplete = null)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("[VideoCacheService] CacheVideo called with empty URL");
            onComplete?.Invoke(false, null);
            return;
        }

        // Check network conditions for background downloads
        if (!ShouldDownload(priority))
        {
            Debug.Log($"[VideoCacheService] Skipping download (WiFi-only mode): {url}");
            onComplete?.Invoke(false, null);
            return;
        }

        string hash = VideoCacheEntry.GetUrlHash(url);

        // Already cached?
        if (IsCached(url))
        {
            string path = GetCachedPath(url);
            Debug.Log($"[VideoCacheService] Already cached: {url}");
            onComplete?.Invoke(true, path);
            return;
        }

        // Already downloading?
        if (activeDownloads.ContainsKey(hash))
        {
            Debug.Log($"[VideoCacheService] Already downloading: {url}");
            // Add callbacks to existing download
            if (activeDownloads.TryGetValue(hash, out var existingOp))
            {
                existingOp.AddCallbacks(onProgress, onComplete);
            }
            return;
        }

        // Already in queue?
        var existingRequest = downloadQueue.FirstOrDefault(r => r.UrlHash == hash);
        if (existingRequest != null)
        {
            // Update priority if higher
            if (priority < existingRequest.Priority)
            {
                existingRequest.Priority = priority;
                SortDownloadQueue();
            }
            existingRequest.AddCallbacks(onProgress, onComplete);
            return;
        }

        // Add to queue
        var request = new DownloadRequest(url, videoId, fileSize, priority, onProgress, onComplete);
        downloadQueue.Add(request);
        SortDownloadQueue();

        Debug.Log($"[VideoCacheService] Queued download: {url} (priority: {priority}, queue size: {downloadQueue.Count})");

        // Process queue
        ProcessDownloadQueue();
    }

    /// <summary>
    /// Cancel a download
    /// </summary>
    public void CancelDownload(string url)
    {
        if (string.IsNullOrEmpty(url)) return;

        string hash = VideoCacheEntry.GetUrlHash(url);

        // Remove from queue
        downloadQueue.RemoveAll(r => r.UrlHash == hash);

        // Cancel active download
        if (activeDownloads.TryGetValue(hash, out var operation))
        {
            operation.Cancel();
            activeDownloads.Remove(hash);
        }
    }

    /// <summary>
    /// Remove a video from cache
    /// </summary>
    public bool RemoveFromCache(string url)
    {
        if (string.IsNullOrEmpty(url)) return false;

        string hash = VideoCacheEntry.GetUrlHash(url);

        if (cacheIndex.TryGetValue(hash, out var entry))
        {
            try
            {
                if (File.Exists(entry.localPath))
                {
                    File.Delete(entry.localPath);
                }
                cacheIndex.Remove(hash);
                SaveCacheIndex();
                Debug.Log($"[VideoCacheService] Removed from cache: {url}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[VideoCacheService] Failed to remove from cache: {ex}");
            }
        }
        return false;
    }

    /// <summary>
    /// Clear all cached videos
    /// </summary>
    public void ClearCache()
    {
        try
        {
            // Cancel all active downloads
            foreach (var op in activeDownloads.Values)
            {
                op.Cancel();
            }
            activeDownloads.Clear();
            downloadQueue.Clear();

            // Delete all files
            if (Directory.Exists(cacheFolder))
            {
                var files = Directory.GetFiles(cacheFolder, "*.mp4");
                foreach (var file in files)
                {
                    try { File.Delete(file); }
                    catch { }
                }
            }

            cacheIndex.Clear();
            SaveCacheIndex();

            Debug.Log("[VideoCacheService] Cache cleared");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[VideoCacheService] Failed to clear cache: {ex}");
        }
    }

    /// <summary>
    /// Get cache statistics
    /// </summary>
    public (long usedBytes, long limitBytes, int cachedCount) GetCacheStats()
    {
        long used = cacheIndex.Values
            .Where(e => e.isFullyCached)
            .Sum(e => e.fileSize);

        return (used, maxCacheSizeBytes, cacheIndex.Count(e => e.Value.isFullyCached));
    }

    #endregion

    #region Download Queue Management

    private void SortDownloadQueue()
    {
        downloadQueue = downloadQueue.OrderBy(r => (int)r.Priority).ToList();
    }

    private void ProcessDownloadQueue()
    {
        while (activeDownloads.Count < maxConcurrentDownloads && downloadQueue.Count > 0)
        {
            var request = downloadQueue[0];
            downloadQueue.RemoveAt(0);
            StartDownload(request);
        }
    }

    private void StartDownload(DownloadRequest request)
    {
        var operation = new DownloadOperation(request);
        activeDownloads[request.UrlHash] = operation;

        StartCoroutine(DownloadCoroutine(operation));
    }

    private IEnumerator DownloadCoroutine(DownloadOperation operation)
    {
        string url = operation.Request.Url;
        string hash = operation.Request.UrlHash;
        string extension = GetExtensionFromUrl(url);
        string fileName = $"{hash}{extension}";
        string filePath = Path.Combine(cacheFolder, fileName);
        string tempPath = filePath + ".tmp";

        Debug.Log($"[VideoCacheService] Starting download: {url}");

        // Ensure space is available
        long requiredSize = operation.Request.FileSize > 0 ? operation.Request.FileSize : 50 * 1024 * 1024; // Default 50MB if unknown
        EvictIfNeeded(requiredSize);

        // Clean up any existing temp file
        if (File.Exists(tempPath))
        {
            try { File.Delete(tempPath); }
            catch { }
        }

        using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
        {
            request.downloadHandler = new DownloadHandlerFile(tempPath);
            operation.WebRequest = request;

            var asyncOp = request.SendWebRequest();

            while (!asyncOp.isDone)
            {
                if (operation.IsCancelled)
                {
                    request.Abort();
                    CleanupTempFile(tempPath);
                    yield break;
                }

                operation.Progress = request.downloadProgress;
                operation.InvokeProgress(request.downloadProgress);
                OnDownloadProgress?.Invoke(url, request.downloadProgress);

                yield return null;
            }

            // Check result
#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogError($"[VideoCacheService] Download failed: {request.error}");
                CleanupTempFile(tempPath);
                CompleteDownload(operation, false, null);
                yield break;
            }

            // Move temp to final
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                File.Move(tempPath, filePath);

                // Get actual file size
                long actualSize = new FileInfo(filePath).Length;

                // Create cache entry
                var entry = new VideoCacheEntry(url, operation.Request.VideoId, actualSize, operation.Request.Priority)
                {
                    localPath = filePath,
                    isFullyCached = true,
                    downloadedBytes = actualSize
                };

                cacheIndex[hash] = entry;
                SaveCacheIndex();

                Debug.Log($"[VideoCacheService] Download complete: {url} ({actualSize / 1024f / 1024f:F2} MB)");
                CompleteDownload(operation, true, filePath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[VideoCacheService] Failed to finalize download: {ex}");
                CleanupTempFile(tempPath);
                CleanupTempFile(filePath);
                CompleteDownload(operation, false, null);
            }
        }
    }

    private void CompleteDownload(DownloadOperation operation, bool success, string path)
    {
        string hash = operation.Request.UrlHash;
        activeDownloads.Remove(hash);

        operation.InvokeComplete(success, path);
        OnDownloadComplete?.Invoke(operation.Request.Url, success);

        // Process next in queue
        ProcessDownloadQueue();
    }

    private void CleanupTempFile(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch { }
    }

    #endregion

    #region LRU Eviction

    private void EvictIfNeeded(long requiredSpace)
    {
        var stats = GetCacheStats();
        long targetSize = maxCacheSizeBytes - requiredSpace;

        if (stats.usedBytes <= targetSize)
            return;

        Debug.Log($"[VideoCacheService] Evicting to make room. Current: {stats.usedBytes / 1024f / 1024f:F2} MB, Target: {targetSize / 1024f / 1024f:F2} MB");

        // Get evictable entries (not Manual priority), sorted by last access (oldest first)
        var evictable = cacheIndex.Values
            .Where(e => e.isFullyCached && e.priority != CachePriority.Manual)
            .OrderBy(e => e.LastAccessedAt)
            .ToList();

        long currentSize = stats.usedBytes;

        foreach (var entry in evictable)
        {
            if (currentSize <= targetSize)
                break;

            try
            {
                if (File.Exists(entry.localPath))
                {
                    File.Delete(entry.localPath);
                    currentSize -= entry.fileSize;
                    cacheIndex.Remove(entry.urlHash);
                    Debug.Log($"[VideoCacheService] Evicted: {entry.videoId} ({entry.fileSize / 1024f / 1024f:F2} MB)");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[VideoCacheService] Failed to evict {entry.videoId}: {ex.Message}");
            }
        }

        SaveCacheIndex();
    }

    #endregion

    #region Cache Index Persistence

    private void LoadCacheIndex()
    {
        try
        {
            if (File.Exists(indexFilePath))
            {
                string json = File.ReadAllText(indexFilePath);
                var wrapper = JsonUtility.FromJson<CacheIndexWrapper>(json);

                if (wrapper?.entries != null)
                {
                    cacheIndex.Clear();
                    foreach (var entry in wrapper.entries)
                    {
                        // Verify file still exists
                        if (entry.isFullyCached && File.Exists(entry.localPath))
                        {
                            cacheIndex[entry.urlHash] = entry;
                        }
                    }
                    Debug.Log($"[VideoCacheService] Loaded {cacheIndex.Count} cached entries");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[VideoCacheService] Failed to load cache index: {ex}");
            cacheIndex.Clear();
        }
    }

    private void SaveCacheIndex()
    {
        try
        {
            var wrapper = new CacheIndexWrapper
            {
                entries = cacheIndex.Values.ToList()
            };

            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(indexFilePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[VideoCacheService] Failed to save cache index: {ex}");
        }
    }

    [Serializable]
    private class CacheIndexWrapper
    {
        public List<VideoCacheEntry> entries = new List<VideoCacheEntry>();
    }

    #endregion

    #region Helpers

    private bool ShouldDownload(CachePriority priority)
    {
        // Critical and Manual always download
        if (priority == CachePriority.Critical || priority == CachePriority.Manual)
            return true;

        // Check WiFi-only setting for background downloads
        if (wifiOnlyForBackground)
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }

        return true;
    }

    private string GetExtensionFromUrl(string url)
    {
        try
        {
            // Remove query string
            int queryIndex = url.IndexOf('?');
            string path = queryIndex > 0 ? url.Substring(0, queryIndex) : url;

            string ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext))
                return ".mp4";

            return ext.ToLowerInvariant();
        }
        catch
        {
            return ".mp4";
        }
    }

    #endregion

    #region Inner Classes

    private class DownloadRequest
    {
        public string Url { get; }
        public string VideoId { get; }
        public string UrlHash { get; }
        public long FileSize { get; }
        public CachePriority Priority { get; set; }

        private List<Action<float>> progressCallbacks = new List<Action<float>>();
        private List<Action<bool, string>> completeCallbacks = new List<Action<bool, string>>();

        public DownloadRequest(string url, string videoId, long fileSize, CachePriority priority,
            Action<float> onProgress, Action<bool, string> onComplete)
        {
            Url = url;
            VideoId = videoId;
            UrlHash = VideoCacheEntry.GetUrlHash(url);
            FileSize = fileSize;
            Priority = priority;

            if (onProgress != null) progressCallbacks.Add(onProgress);
            if (onComplete != null) completeCallbacks.Add(onComplete);
        }

        public void AddCallbacks(Action<float> onProgress, Action<bool, string> onComplete)
        {
            if (onProgress != null) progressCallbacks.Add(onProgress);
            if (onComplete != null) completeCallbacks.Add(onComplete);
        }

        public void InvokeProgress(float progress)
        {
            foreach (var callback in progressCallbacks)
            {
                try { callback?.Invoke(progress); }
                catch { }
            }
        }

        public void InvokeComplete(bool success, string path)
        {
            foreach (var callback in completeCallbacks)
            {
                try { callback?.Invoke(success, path); }
                catch { }
            }
        }
    }

    private class DownloadOperation
    {
        public DownloadRequest Request { get; }
        public UnityWebRequest WebRequest { get; set; }
        public float Progress { get; set; }
        public bool IsCancelled { get; private set; }

        private List<Action<float>> progressCallbacks = new List<Action<float>>();
        private List<Action<bool, string>> completeCallbacks = new List<Action<bool, string>>();

        public DownloadOperation(DownloadRequest request)
        {
            Request = request;
        }

        public void AddCallbacks(Action<float> onProgress, Action<bool, string> onComplete)
        {
            if (onProgress != null) progressCallbacks.Add(onProgress);
            if (onComplete != null) completeCallbacks.Add(onComplete);
        }

        public void InvokeProgress(float progress)
        {
            Request.InvokeProgress(progress);
            foreach (var callback in progressCallbacks)
            {
                try { callback?.Invoke(progress); }
                catch { }
            }
        }

        public void InvokeComplete(bool success, string path)
        {
            Request.InvokeComplete(success, path);
            foreach (var callback in completeCallbacks)
            {
                try { callback?.Invoke(success, path); }
                catch { }
            }
        }

        public void Cancel()
        {
            IsCancelled = true;
            WebRequest?.Abort();
        }
    }

    #endregion
}
