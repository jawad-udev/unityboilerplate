using Backend;
using UnityEngine;

/// <summary>
/// Centralized cache policy configuration for all API endpoints.
/// Simplifies cache decisions across the entire app.
/// Usage: var policy = CachePolicy.GetRecentMedia; MediaAPI.GetRecentMedia(..., loadFromCache: policy.cache, cacheType: policy.type, ttlSeconds: policy.ttl);
/// </summary>
public static class CachePolicy
{
    // ==================== TTL Constants ====================
    public static class TTL
    {
        public const int VeryShort = 60;          // 1 minute (real-time data like appointments)
        public const int Short = 120;             // 2 minutes
        public const int Standard = 300;          // 5 minutes (most content)
        public const int Long = 600;              // 10 minutes (search results)
        public const int VeryLong = 1800;         // 30 minutes (user profile)
        public const int Persistent = 3600;       // 1 hour (settings)
        public const int OnDay = 86400;           // 1 day (static data)
    }

    // ==================== Cache Result Structure ====================
    public struct CachePolicyResult
    {
        public bool shouldCache;
        public CacheType cacheType;
        public int ttlSeconds;

        public CachePolicyResult(bool cache, CacheType type, int ttl)
        {
            shouldCache = cache;
            cacheType = type;
            ttlSeconds = ttl;
        }
    }

    // ==================== MEDIA APIS ====================

    /// <summary>
    /// Recent media (paginated) - changes often, cache for UX
    /// </summary>
    public static CachePolicyResult GetRecentMedia =>
        new CachePolicyResult(true, CacheType.Session, TTL.Standard);

    /// <summary>
    /// Trending media - popular content, cache for performance
    /// </summary>
    public static CachePolicyResult GetTrendingMedia =>
        new CachePolicyResult(true, CacheType.Session, TTL.Standard);

    /// <summary>
    /// Public media - static list, cache aggressively
    /// </summary>
    public static CachePolicyResult GetPublicMedia =>
        new CachePolicyResult(true, CacheType.Session, TTL.Long);

    /// <summary>
    /// My recent media - user-specific, cache briefly
    /// </summary>
    public static CachePolicyResult GetMyRecentMedia =>
        new CachePolicyResult(true, CacheType.Session, TTL.Standard);

    /// <summary>
    /// Search media - expensive query, cache longer
    /// </summary>
    public static CachePolicyResult SearchMedia =>
        new CachePolicyResult(true, CacheType.Session, TTL.Long);

    /// <summary>
    /// Liked media - user-modified, cache briefly
    /// </summary>
    public static CachePolicyResult GetLikedMedia =>
        new CachePolicyResult(true, CacheType.Session, TTL.Standard);

    /// <summary>
    /// Get single media details - referenced often, cache
    /// </summary>
    public static CachePolicyResult GetMedia =>
        new CachePolicyResult(true, CacheType.Session, TTL.Long);

    /// <summary>
    /// Media by age group - semi-static, cache
    /// </summary>
    public static CachePolicyResult GetMediaByAgeGroup =>
        new CachePolicyResult(true, CacheType.Session, TTL.Long);

    /// <summary>
    /// RMT media - semi-static, cache
    /// </summary>
    public static CachePolicyResult GetRmtMedia =>
        new CachePolicyResult(true, CacheType.Session, TTL.Standard);

    // ==================== PLAYLIST APIS ====================

    /// <summary>
    /// Get all playlists - user list, cache briefly
    /// </summary>
    public static CachePolicyResult GetPlaylists =>
        new CachePolicyResult(true, CacheType.Session, TTL.Standard);

    /// <summary>
    /// Get single playlist - referenced often, cache longer
    /// </summary>
    public static CachePolicyResult GetPlaylist =>
        new CachePolicyResult(true, CacheType.Session, TTL.Long);

    /// <summary>
    /// Get user playlists - user list, cache briefly
    /// </summary>
    public static CachePolicyResult GetUserPlaylists =>
        new CachePolicyResult(true, CacheType.Session, TTL.Standard);

    // ==================== RMT APIS ====================

    /// <summary>
    /// Get all RMTs - semi-static list, cache
    /// </summary>
    public static CachePolicyResult GetRmts =>
        new CachePolicyResult(true, CacheType.Session, TTL.Standard);

    /// <summary>
    /// Recommended RMTs - personalized, cache briefly
    /// </summary>
    public static CachePolicyResult GetRecommendedRmts =>
        new CachePolicyResult(true, CacheType.Session, TTL.Long);

    /// <summary>
    /// Available RMTs - time-dependent, cache briefly
    /// </summary>
    public static CachePolicyResult GetAvailableRmts =>
        new CachePolicyResult(true, CacheType.Session, TTL.Long);

    /// <summary>
    /// RMT details - referenced often, cache
    /// </summary>
    public static CachePolicyResult GetRmtDetails =>
        new CachePolicyResult(true, CacheType.Session, TTL.Long);

    /// <summary>
    /// My subscribed RMTs - user list, cache briefly
    /// </summary>
    public static CachePolicyResult GetMySubscribedRmts =>
        new CachePolicyResult(true, CacheType.Session, TTL.Standard);

    /// <summary>
    /// RMT data lists (counties, credentials, etc) - static data, cache persistently
    /// </summary>
    public static CachePolicyResult GetRmtsDataLists =>
        new CachePolicyResult(true, CacheType.Persistent, TTL.OnDay);

    // ==================== USER/CONSUMER APIS ====================

    /// <summary>
    /// Get consumer profile - rarely changes, cache persistently
    /// </summary>
    public static CachePolicyResult GetConsumers =>
        new CachePolicyResult(true, CacheType.Session, TTL.OnDay);

    /// <summary>
    /// Connected RMTs - user list, cache briefly
    /// </summary>
    public static CachePolicyResult GetConnectedRmts =>
        new CachePolicyResult(true, CacheType.Session, TTL.Standard);

    /// <summary>
    /// User settings - very stable, cache persistently
    /// </summary>
    public static CachePolicyResult GetSettings =>
        new CachePolicyResult(true, CacheType.Persistent, TTL.Persistent);

    // ==================== APPOINTMENT APIS ====================

    /// <summary>
    /// Upcoming appointments - time-sensitive, short cache
    /// </summary>
    public static CachePolicyResult GetUpcomingAppointments =>
        new CachePolicyResult(true, CacheType.Session, TTL.VeryShort);

    /// <summary>
    /// Consumer appointments - time-sensitive, short cache
    /// </summary>
    public static CachePolicyResult GetConsumerAppointments =>
        new CachePolicyResult(true, CacheType.Session, TTL.VeryShort);

    /// <summary>
    /// Appointment availability - critical for UX, short cache
    /// </summary>
    public static CachePolicyResult GetAppointmentAvailability =>
        new CachePolicyResult(true, CacheType.Session, TTL.Short);

    // ==================== NO-CACHE ENDPOINTS ====================
    // These are mutations (POST/PUT/PATCH/DELETE) - NEVER cache

    public static readonly CachePolicyResult DoNotCache =
        new CachePolicyResult(false, CacheType.Session, -1);

    /// <summary>
    /// Use this for all mutation operations:
    /// - LikeToggle, SaveToggle
    /// - CreatePlaylist, UpdatePlaylist, DeletePlaylist, AddMediaToPlaylist, RemoveMediaFromPlaylist
    /// - UpdateMediaPatch, UpdateMediaPut, DeleteMediaAction
    /// - UpdateConsumerProfile, PatchConsumerProfile, UpdateSettings
    /// - SubscribeToRmt, UnsubscribeFromRmt, UpdateRmtProfile
    /// - BookAppointment
    /// - All upload/confirmation operations
    /// </summary>
    public static CachePolicyResult MutationOperation => DoNotCache;

    // ==================== UTILITY METHODS ====================

    /// <summary>
    /// Apply cache policy to an API call
    /// Usage: var policy = CachePolicy.GetRecentMedia;
    ///        MediaAPI.GetRecentMedia(..., loadFromCache: policy.shouldCache, cacheType: policy.cacheType, ttlSeconds: policy.ttlSeconds);
    /// </summary>
    public static string GetPolicyDescription(CachePolicyResult policy)
    {
        if (!policy.shouldCache)
            return "No caching (mutation or real-time operation)";

        string typeStr = policy.cacheType == CacheType.Session ? "Session" : "Persistent";
        return $"{typeStr} cache - TTL: {policy.ttlSeconds}s ({GetTTLLabel(policy.ttlSeconds)})";
    }

    private static string GetTTLLabel(int ttl)
    {
        if (ttl == TTL.VeryShort) return "1 min";
        if (ttl == TTL.Short) return "2 min";
        if (ttl == TTL.Standard) return "5 min";
        if (ttl == TTL.Long) return "10 min";
        if (ttl == TTL.VeryLong) return "30 min";
        if (ttl == TTL.Persistent) return "1 hour";
        if (ttl == TTL.OnDay) return "1 day";
        return $"{ttl}s";
    }

    /// <summary>
    /// Clear all session caches (e.g., on manual refresh or pull-to-refresh)
    /// </summary>
    public static void ClearSessionCaches()
    {
        Debug.Log("[CachePolicy] Clearing all session caches");
        ApiCacheManager.ClearSession();
    }

    /// <summary>
    /// Clear persistent caches (e.g., on logout)
    /// </summary>
    public static void ClearPersistentCaches()
    {
        Debug.Log("[CachePolicy] Clearing all persistent caches");
        ApiCacheManager.ClearPersistent();
    }

    /// <summary>
    /// Clear all caches (logout scenario)
    /// </summary>
    public static void ClearAllCaches()
    {
        Debug.Log("[CachePolicy] Clearing all caches (logout)");
        ClearSessionCaches();
        ClearPersistentCaches();
    }

    /// <summary>
    /// Invalidate user playlists cache (call after create/delete/update playlist)
    /// </summary>
    public static void InvalidateUserPlaylists()
    {
        InvalidateCache("api/v1/playlists/user-playlists/");
    }

    /// <summary>
    /// Invalidate public playlists cache
    /// </summary>
    public static void InvalidatePlaylists()
    {
        InvalidateCache("api/v1/playlists/");
    }

    /// <summary>
    /// Invalidate a specific playlist's cache (call after add/remove media from playlist)
    /// </summary>
    public static void InvalidatePlaylist(string playlistId)
    {
        if (string.IsNullOrEmpty(playlistId)) return;

        // The request path is the full URL, and for GET requests body is null
        // Cache key format: fullUrl + "_" + body (body is null for GET, so key ends with "_")
        string fullUrl = GameClient.Instance._hostUrl + $"api/v1/playlists/{playlistId}/";
        string cacheKey = fullUrl + "_";

        Debug.Log($"[CachePolicy] Invalidating playlist cache. Key: {cacheKey}");
        ApiCacheManager.Remove(cacheKey, CacheType.Session);
    }

    /// <summary>
    /// Invalidate liked media cache (call after like/unlike)
    /// </summary>
    public static void InvalidateLikedMedia()
    {
        ApiCacheManager.RemoveByUrlPrefix("api/v1/media/liked/");
        Debug.Log("[CachePolicy] Invalidated liked media cache");
    }

    /// <summary>
    /// Invalidate subscribed RMTs cache (call after subscribe/unsubscribe)
    /// </summary>
    public static void InvalidateSubscribedRmts()
    {
        ApiCacheManager.RemoveByUrlPrefix("api/v1/rmt-subscribe/my-list/");
        Debug.Log("[CachePolicy] Invalidated subscribed RMTs cache");
    }

    /// <summary>
    /// Generic cache invalidation helper
    /// </summary>
    private static void InvalidateCache(string endpoint, CacheType cacheType = CacheType.Session)
    {
        // Use prefix-based invalidation to handle URLs with query parameters
        ApiCacheManager.RemoveByUrlPrefix(endpoint);
        Debug.Log($"[CachePolicy] Invalidated cache: {endpoint}");
    }
}
