using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public class HealthAPI
    {
        public static void GetHealthDetailed(Action<ResponseMessage<object>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "health-detailed/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.GET,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }
    }
}
