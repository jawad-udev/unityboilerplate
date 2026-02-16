using System;
using System.Collections.Generic;

namespace Backend
{
    public static class MediaAPI
    {
        private static void AddCommonHeaders(RequestMessage req)
        {
            req._headers = RequestMessage._defaultHeaders;
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
        }

        // GET /api/v1/media/{media_id}/
        public static void GetMedia(string mediaId, Action<ResponseMessage<MediaResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetMedia;
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/{mediaId}/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // PUT /api/v1/media/actions/{id}/
        public static void UpdateMediaPut(string id, MediaActionResponse payload, Action<ResponseMessage<MediaActionResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/actions/{id}/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.PUT,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _body = payload.ToJson()
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // PATCH /api/v1/media/actions/{id}/
        public static void UpdateMediaPatch(string id, Dictionary<string, object> patchBody, Action<ResponseMessage<MediaActionResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/actions/{id}/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.PATCH,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _body = GameSerializer.Serialize(patchBody)
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // DELETE /api/v1/media/actions/{id}/
        public static void DeleteMediaAction(string id, Action<ResponseMessage<object>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/actions/{id}/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.DELETE,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // GET /api/v1/media/age-group/{age_group_id}/
        public static void GetMediaByAgeGroup(int ageGroupId, int limit, int offset, Action<ResponseMessage<PaginatedMediaResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetMediaByAgeGroup;
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/age-group/{ageGroupId}/?limit={limit}&offset={offset}";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // GET /api/v1/media/my-media/
        public static void GetMyMedia(int limit, int offset, Action<ResponseMessage<PaginatedMediaResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetMyRecentMedia;
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/my-media/?limit={limit}&offset={offset}";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // GET /api/v1/media/outcome/{outcomes}/
        public static void GetMediaByOutcome(string outcomesCsv, int limit, int offset, Action<ResponseMessage<PaginatedMediaResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetPublicMedia;
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/outcome/{Uri.EscapeDataString(outcomesCsv)}/?limit={limit}&offset={offset}";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // GET /api/v1/media/public/
        public static void GetPublicMedia(int limit, int offset, Action<ResponseMessage<PaginatedMediaResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetPublicMedia;
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/public/?limit={limit}&offset={offset}";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // GET /api/v1/media/recent/
        public static void GetRecentMedia(int limit, int offset, Action<ResponseMessage<PaginatedMediaResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetRecentMedia;
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/recent/?limit={limit}&offset={offset}";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // GET /api/v1/media/consumer/my-recent/
        public static void GetMyRecentMedia(int limit, int offset, Action<ResponseMessage<List<MediaResponse>>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetMyRecentMedia;
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/consumer/my-recent/?limit={limit}&offset={offset}";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // GET /api/v1/media/consumer/trending/
        public static void GetTrendingMedia(int limit, int offset, Action<ResponseMessage<List<MediaResponse>>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetTrendingMedia;
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/consumer/trending/?limit={limit}&offset={offset}";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // GET /api/v1/media/rmt/{rmt_id}/
        public static void GetRmtMedia(int rmtId, string mediaType, int limit, int offset, Action<ResponseMessage<PaginatedRmtMediaResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetRmtMedia;
            var qs = new List<string>() { $"limit={limit}", $"offset={offset}" };
            if (!string.IsNullOrEmpty(mediaType)) qs.Add($"media_type={Uri.EscapeDataString(mediaType)}");
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/rmt/{rmtId}/?{string.Join("&", qs)}";

            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        public static void SearchMedia(
        int? ageGroup = null,
        int? limit = null,
        int? offset = null,
        string outcomesCsv = null,
        int? owner = null,
        string q = null,
        string mediaTypeCsv = null,
        bool? hasActiveIntervention = null,
        Action<ResponseMessage<PaginatedMediaResponse>> listener = null,
        bool? loadFromCache = null)
        {
            var policy = CachePolicy.SearchMedia;
            var qs = new List<string>();

            if (limit.HasValue) qs.Add($"limit={limit.Value}");
            if (offset.HasValue) qs.Add($"offset={offset.Value}");
            if (ageGroup.HasValue) qs.Add($"age_group={ageGroup.Value}");
            if (!string.IsNullOrEmpty(outcomesCsv)) qs.Add($"outcomes={Uri.EscapeDataString(outcomesCsv)}");
            if (owner.HasValue) qs.Add($"owner={owner.Value}");
            if (!string.IsNullOrEmpty(q)) qs.Add($"q={Uri.EscapeDataString(q)}");
            if (!string.IsNullOrEmpty(mediaTypeCsv)) qs.Add($"media_type={Uri.EscapeDataString(mediaTypeCsv)}");
            if (hasActiveIntervention.HasValue) qs.Add($"has_active_intervention={hasActiveIntervention.Value.ToString().ToLower()}");

            string queryString = qs.Count > 0 ? $"?{string.Join("&", qs)}" : string.Empty;
            string requestPath = $"{GameClient.Instance._hostUrl}api/v1/media/search/{queryString}";

            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // POST /api/v1/media/uploads/session/
        public static void CreateUploadSession(MediaUploadSessionRequest request, Action<ResponseMessage<MediaUploadSessionResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/media/uploads/session/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _body = GameSerializer.Serialize(request)
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // POST /api/v1/media/uploads/{upload_uuid}/confirm/
        public static void ConfirmUpload(string uploadUuid, MediaUploadConfirmRequest request, Action<ResponseMessage<MediaUploadConfirmResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/uploads/{uploadUuid}/confirm/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _body = GameSerializer.Serialize(request)
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // POST /api/v1/actions/media/{media_id}/save-toggle/
        public static void SaveToggle(string mediaId, Action<ResponseMessage<object>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/actions/media/{mediaId}/save-toggle/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // POST /api/v1/actions/media/{media_id}/like-toggle/
        public static void LikeToggle(string mediaId, Action<ResponseMessage<object>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/actions/media/{mediaId}/like-toggle/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // GET /api/v1/media/liked/
        public static void GetLikedMedia(int limit, int offset, string mediaTypeCsv, Action<ResponseMessage<PaginatedMediaResponse>> listener)
        {
            var policy = CachePolicy.GetLikedMedia;
            var qs = new List<string>() { $"limit={limit}", $"offset={offset}" };
            if (!string.IsNullOrEmpty(mediaTypeCsv))
                qs.Add($"media_type={Uri.EscapeDataString(mediaTypeCsv)}");

            string requestPath = GameClient.Instance._hostUrl + $"api/v1/media/liked/?{string.Join("&", qs)}";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }
    }
}
