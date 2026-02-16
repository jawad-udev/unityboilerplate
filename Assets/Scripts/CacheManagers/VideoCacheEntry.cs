using System;

/// <summary>
/// Represents a cached video entry with metadata for LRU eviction
/// </summary>
[Serializable]
public class VideoCacheEntry
{
    public string videoId;           // MediaResponse.id
    public string sourceUrl;         // Original URL
    public string localPath;         // Local file path
    public string urlHash;           // Hash of URL for filename
    public long fileSize;            // Size in bytes
    public float duration;           // Duration in seconds
    public long cachedAtTicks;       // When cached (DateTime.Ticks for serialization)
    public long lastAccessedAtTicks; // For LRU (DateTime.Ticks for serialization)
    public bool isFullyCached;       // Download complete?
    public long downloadedBytes;     // For tracking partial downloads
    public CachePriority priority;   // Download priority

    // Non-serialized accessors for DateTime
    public DateTime CachedAt
    {
        get => new DateTime(cachedAtTicks);
        set => cachedAtTicks = value.Ticks;
    }

    public DateTime LastAccessedAt
    {
        get => new DateTime(lastAccessedAtTicks);
        set => lastAccessedAtTicks = value.Ticks;
    }

    public VideoCacheEntry()
    {
        CachedAt = DateTime.UtcNow;
        LastAccessedAt = DateTime.UtcNow;
    }

    public VideoCacheEntry(string url, string videoId, long fileSize, CachePriority priority)
    {
        this.sourceUrl = url;
        this.videoId = videoId;
        this.fileSize = fileSize;
        this.priority = priority;
        this.urlHash = GetUrlHash(url);
        this.isFullyCached = false;
        this.downloadedBytes = 0;
        CachedAt = DateTime.UtcNow;
        LastAccessedAt = DateTime.UtcNow;
    }

    public static string GetUrlHash(string url)
    {
        if (string.IsNullOrEmpty(url)) return "unknown";

        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(url);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    public void MarkAccessed()
    {
        LastAccessedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Priority levels for video caching
/// Lower value = higher priority
/// </summary>
public enum CachePriority
{
    Critical = 0,   // Currently playing - highest priority
    High = 1,       // Next in queue / adjacent in carousel
    Normal = 2,     // Visible in feed
    Low = 3,        // Background prefetch
    Manual = 4      // User requested offline - never auto-evict
}
