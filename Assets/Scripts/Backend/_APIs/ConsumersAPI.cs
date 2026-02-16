using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public class ConsumersAPI
    {
        public static void GetConsumers(Action<ResponseMessage<UserInfo>> listener)
        {
            var policy = CachePolicy.GetConsumers;
            string requestPath = GameClient.Instance._hostUrl + "api/v1/consumers/";
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
                policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        public static void UpdateConsumerProfile(UpdateConsumerProfileRequest request, Action<ResponseMessage<UpdateConsumerProfileResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/consumers/profile/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.PUT,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(request)
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void PatchConsumerProfile(Dictionary<string, object> patchData, Action<ResponseMessage<UpdateConsumerProfileResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/consumers/profile/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.PATCH,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(patchData)
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        // GET connected RMTs (paginated)
        public static void GetConnectedRmts(int limit, int offset, Action<ResponseMessage<PaginatedConnectedRmtsResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetConnectedRmts;
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/consumers/rmts/connected/?limit={limit}&offset={offset}";
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

        // GET User Settings
        public static void GetSettings(Action<ResponseMessage<UserSettingsData>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetSettings;
            string requestPath = GameClient.Instance._hostUrl + "api/v1/users/settings/";
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

        // PUT Update User Settings
        public static void UpdateSettings(UserSettingsData settingsData, Action<ResponseMessage<UserSettingsData>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/users/settings/";

            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.PUT,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(settingsData)
            };

            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);

            GameClient.Instance.DispatchRequest(req, listener);
        }
    }
}
