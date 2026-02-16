using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public class ClientsAPI
    {
        public static void AddOfflineClient(string fullName, string email, Action<ResponseMessage<AddOfflineClientResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/clients/add/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "full_name", fullName },
                    { "email", email }
                })
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void GetClients(Action<ResponseMessage<List<RmtClientResponse>>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/clients/get/";
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
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void RedeemInvitation(string token, int clientId, Action<ResponseMessage<InvitationRedeemResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/clients/invitation/redeem/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "token", token },
                    { "client_id", clientId }
                })
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }
    }
}
