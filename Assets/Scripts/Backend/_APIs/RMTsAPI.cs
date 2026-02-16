using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public class RMTsAPI
    {
        private static void AddCommonHeaders(RequestMessage req)
        {
            req._headers = RequestMessage._defaultHeaders;
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
        }

        public static void GetRmts(Action<ResponseMessage<RmtUser>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetRmts;
            string requestPath = GameClient.Instance._hostUrl + "api/v1/rmts/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // GET /api/v1/rmts/data-lists
        public static void GetRmtsDataLists(Action<ResponseMessage<Backend.RmtDataListsResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetRmtsDataLists;
            string requestPath = GameClient.Instance._hostUrl + "api/v1/rmts/data-lists";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders
            };

            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);

            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        public static void UpdateRmtProfile(UpdateRMTProfileRequest request, Action<ResponseMessage<UpdateRMTProfileResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/rmts/profile/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.PATCH,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = request.ToJson()
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // POST /api/v1/rmts/credentials/uploads/session/
        public static void CreateUploadSession(CredentialUploadSessionRequest request, Action<ResponseMessage<CredentialUploadSessionResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/rmts/credentials/uploads/session/";

            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _body = request.ToJson()
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // POST /api/v1/rmts/credentials/uploads/{upload_uuid}/confirm/
        public static void ConfirmUpload(string uploadUuid, CredentialUploadConfirmRequest request, Action<ResponseMessage<CredentialUploadConfirmResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/rmts/credentials/uploads/{uploadUuid}/confirm/";

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

        // GET /api/v1/rmts/consumer/recommended
        public static void GetRecommendedRmts(Action<ResponseMessage<RecommendedRmtsResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetRecommendedRmts;
            string requestPath = GameClient.Instance._hostUrl + "api/v1/rmts/consumer/recommended";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // GET /api/v1/rmts/consumer/available
        public static void GetAvailableRmts(Action<ResponseMessage<AvailableRmtsResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetAvailableRmts;
            string requestPath = GameClient.Instance._hostUrl + "api/v1/rmts/consumer/available";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // GET /api/v1/rmt-subscribe/my-list/ (paginated)
        public static void GetMySubscribedRmts(int limit, int offset, Action<ResponseMessage<PaginatedConnectedRmtsResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetMySubscribedRmts;
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/rmt-subscribe/my-list/?limit={limit}&offset={offset}";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // POST /api/v1/rmt-subscribe/{rmt_id}/
        public static void SubscribeToRmt(int rmtId, Action<ResponseMessage<RmtSubscriptionResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/rmt-subscribe/{rmtId}/";

            var requestBody = new { rmt = rmtId };

            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _body = GameSerializer.Serialize(requestBody)
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // DELETE /api/v1/rmt-subscribe/{rmt_id}/
        public static void UnsubscribeFromRmt(int rmtId, Action<ResponseMessage<object>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/rmt-subscribe/{rmtId}/";

            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.DELETE,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // GET /api/v1/rmts/{rmt_id}/
        public static void GetRmtDetails(int rmtId, Action<ResponseMessage<RmtDetailResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetRmtDetails;
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/rmts/{rmtId}/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

    }
}
