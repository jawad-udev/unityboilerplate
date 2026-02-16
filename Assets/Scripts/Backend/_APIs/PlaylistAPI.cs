using System;
using System.Collections.Generic;

namespace Backend
{
    public static class PlaylistAPI
    {
        private static void AddCommonHeaders(RequestMessage req)
        {
            req._headers = RequestMessage._defaultHeaders;
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
        }

        // GET /api/v1/playlists/
        public static void GetPlaylists(Action<ResponseMessage<List<PlaylistSummaryResponse>>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetPlaylists;
            string requestPath = GameClient.Instance._hostUrl + "api/v1/playlists/";
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

        // POST /api/v1/playlists/
        public static void CreatePlaylist(PlaylistCreateRequest request, Action<ResponseMessage<PlaylistCreateResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/playlists/";
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

        // GET /api/v1/playlists/{id}/
        public static void GetPlaylist(string id, Action<ResponseMessage<PlaylistDetailResponse>> listener)
        {
            var policy = CachePolicy.GetPlaylist;
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/playlists/{id}/";
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

        // PUT /api/v1/playlists/{id}/
        public static void UpdatePlaylist(string id, PlaylistUpdateRequest request, Action<ResponseMessage<PlaylistDetailResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/playlists/{id}/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.PATCH,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _body = request.ToJson()
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // PATCH /api/v1/playlists/{id}/
        public static void PatchPlaylist(string id, Dictionary<string, object> patchBody, Action<ResponseMessage<PlaylistDetailResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/playlists/{id}/";
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

        // DELETE /api/v1/playlists/{id}/
        public static void DeletePlaylist(string id, Action<ResponseMessage<object>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/playlists/{id}/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.DELETE,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // POST /api/v1/playlists/{id}/media/add/
        public static void AddMediaToPlaylist(string playlistId, PlaylistMediaModifyRequest request, Action<ResponseMessage<List<PlaylistMediaModifyResponse>>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/playlists/{playlistId}/media/add/";
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

        // POST /api/v1/playlists/{id}/media/remove/
        public static void RemoveMediaFromPlaylist(string playlistId, PlaylistMediaModifyRequest request, Action<ResponseMessage<PlaylistMediaModifyResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/playlists/{playlistId}/media/remove/";
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

        // POST /api/v1/playlists/{id}/media/reorder/
        public static void ReorderPlaylistMedia(string playlistId, PlaylistReorderRequest request, Action<ResponseMessage<PlaylistMediaReorderResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/playlists/{playlistId}/media/reorder/";
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

        // POST /api/v1/playlists/uploads/session/
        public static void CreatePlaylistUploadSession(PlaylistUploadSessionRequest request, Action<ResponseMessage<PlaylistUploadSessionResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/playlists/uploads/session/";
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

        public static void ConfirmPlaylistUploadSession(
        string playlist_id,
        string upload_uuid,
        PlaylistUploadConfirmRequest request,
        Action<ResponseMessage<PlaylistUploadConfirmRequest>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl +
                                 $"api/v1/playlists/{playlist_id}/cover-image/{upload_uuid}/confirm/";

            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _requestPath = requestPath,
                _body = GameSerializer.Serialize(request)
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // GET /api/v1/playlists/user-playlists/
        public static void GetUserPlaylists(Action<ResponseMessage<List<PlaylistSummaryResponse>>> listener)
        {
            var policy = CachePolicy.GetUserPlaylists;
            string requestPath = GameClient.Instance._hostUrl + "api/v1/playlists/user-playlists/";
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
