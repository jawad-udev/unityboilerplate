using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Backend
{
    [Serializable]
    // Should have used builder pattern to construct objects of this class... Meh...
    public class RequestMessage
    {
        public const string KEY_PAYLOAD = "_payload";
        private const string KEY_DEVICE_ID = "deviceId";
        private const string KEY_DEVICE_VERSION = "X-Forwarded-Version";
        private const string KEY_USER_AUTH = "Authorization";
        private const string KEY_PLATFORM = "PLATFORM";
        private const string APP_TOKEN = "X-App-Token";

        public enum RequestType : int
        {
            PUT = 0x0001 << 0,
            GET = 0x0001 << 1,
            POST = 0x0001 << 2,
            DELETE = 0x0001 << 3,
            PATCH = 0x0001 << 4
        }


        private static Dictionary<string, string> defaultHeaders;
        public static Dictionary<string, string> _defaultHeaders
        {
            get
            {
                if (defaultHeaders == null)
                {
                    var config = GameConfig.Instance;
                    defaultHeaders = new Dictionary<string, string>();
                    defaultHeaders[KEY_DEVICE_ID] = SystemInfo.deviceUniqueIdentifier;
                    defaultHeaders[KEY_DEVICE_VERSION] = config.gameVersionCode;
                    defaultHeaders[KEY_PLATFORM] = "MOBILE";
                    defaultHeaders[APP_TOKEN] = config.defaultRequestAppToken;
                }

                return new Dictionary<string, string>(defaultHeaders);
            }
        }

        public int _timeout = 10000; // 20 seconds
        public string _body;
        public string _payload;
        public string _requestPath;
        public RequestType _requestType;
        public Dictionary<string, string> _headers;
        public Dictionary<string, object> _requestParameters = new Dictionary<string, object>();


        public override string ToString()
        {
            return string.Format("[RequestMessage] _requestPath = {0} headers = {1} parameters = {2} body = {3}", _requestPath, GameSerializer.Serialize(_headers),
                GameSerializer.Serialize(_requestParameters), _body);
        }

        // Builder Class
        public class Builder
        {
            private RequestMessage requestMessage;

            public Builder()
            {
                requestMessage = new RequestMessage();
                requestMessage._headers = _defaultHeaders;
            }

            public Builder SetTimeout(int timeout)
            {
                requestMessage._timeout = timeout;
                return this;
            }

            public Builder SetBody(string body)
            {
                requestMessage._body = body;
                return this;
            }

            public Builder SetPayload(string payload)
            {
                requestMessage._payload = payload;
                return this;
            }

            public Builder SetRequestPath(string requestPath)
            {
                requestMessage._requestPath = requestPath;
                return this;
            }

            public Builder SetRequestType(RequestType requestType)
            {
                requestMessage._requestType = requestType;
                return this;
            }

            public Builder AddHeader(string key, string value)
            {
                requestMessage._headers[key] = value;
                return this;
            }

            public Builder AddParameter(string key, object value)
            {
                requestMessage._requestParameters[key] = value;
                return this;
            }

            public RequestMessage Build()
            {
                return requestMessage;
            }
        }
    }
}

