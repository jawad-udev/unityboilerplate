using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Backend
{
    public class RequestDispatcher : MonoBehaviour
    {
        public static DateTime _lastResponseTime = DateTime.Now;

        [SerializeField]
        private RequestMessage _request;

        public void Request(RequestMessage request, Action<GameResponse> callback)
        {
            PrepareUrlWithParameters(request);
            _request = request;
            StartCoroutine(SendRequest(request, callback));
        }

        private IEnumerator SendRequest(RequestMessage request, Action<GameResponse> callback)
        {
            Debug.Log("Sending req = " + request.ToString());

            string method = request._requestType.ToString();
            UnityWebRequest webRequest;

            // Build body for POST/PUT/PATCH
            byte[] bodyRaw = null;
            if ((request._requestType == RequestMessage.RequestType.POST ||
                 request._requestType == RequestMessage.RequestType.PUT ||
                 request._requestType == RequestMessage.RequestType.PATCH) &&
                !string.IsNullOrEmpty(request._body))
            {
                bodyRaw = System.Text.Encoding.UTF8.GetBytes(request._body);
            }

            webRequest = new UnityWebRequest(request._requestPath, method);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            if (bodyRaw != null)
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.timeout = request._timeout / 1000; // UnityWebRequest uses seconds

            // Apply custom headers
            foreach (var pair in request._headers)
            {
                try
                {
                    webRequest.SetRequestHeader(pair.Key, pair.Value);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Could not set header '{pair.Key}': {ex.Message}");
                }
            }

            yield return webRequest.SendWebRequest();

            GameResponse response = ParseResponse(webRequest, request);
            response._request = request;

            Debug.Log("Response = " + GameSerializer.Serialize(response));

            callback?.Invoke(response);
            Destroy(gameObject);
        }

        private GameResponse ParseResponse(UnityWebRequest webRequest, RequestMessage request)
        {
            string responseText = webRequest.downloadHandler?.text ?? "";

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                return ParseSuccessResponse(webRequest, responseText);
            }
            else
            {
                return ParseErrorResponse(webRequest, responseText);
            }
        }

        private GameResponse ParseSuccessResponse(UnityWebRequest webRequest, string responseText)
        {
            Debug.Log("strResponse = " + responseText);

            GameResponse response;
            try
            {
                var des = GameSerializer.Deserialize<GameResponse>(responseText);
                response = des ?? new GameResponse() { data = null };
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Deserialization to TobyTalkResponse failed: " + ex.Message);
                response = new GameResponse() { data = null, message = responseText };
            }

            response.statusCode = (int)webRequest.responseCode;

            // Copy response headers
            try
            {
                var headers = webRequest.GetResponseHeaders();
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        response._headers[header.Key] = header.Value;
                    }

                    if (headers.ContainsKey(RequestMessage.KEY_PAYLOAD))
                        response._payload = headers[RequestMessage.KEY_PAYLOAD];
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Failed copying response headers: " + ex.Message);
            }

            // Update timestamp if present
            try
            {
                _lastResponseTime = Extensions.UnixTimeStampToDateTime(response._timeStamp);
            }
            catch { /* ignore timestamp issues */ }

            return response;
        }

        private GameResponse ParseErrorResponse(UnityWebRequest webRequest, string responseText)
        {
            Debug.Log($"Request error ({webRequest.result}): {webRequest.error}");

            if (string.IsNullOrEmpty(responseText))
            {
                return new GameResponse()
                {
                    data = null,
                    statusCode = (int)webRequest.responseCode,
                    error = webRequest.error
                };
            }

            Debug.Log("Error Response JSON: " + responseText);

            try
            {
                var errorObj = JsonConvert.DeserializeObject<ErrorResponse>(responseText);

                string finalError = errorObj?.Error;
                if (errorObj?.Data != null && errorObj.Data.Count > 0)
                {
                    var firstEntry = errorObj.Data.First();
                    if (firstEntry.Value != null && firstEntry.Value.Length > 0)
                    {
                        finalError = firstEntry.Value[0];
                    }
                }

                return new GameResponse()
                {
                    statusCode = (int)webRequest.responseCode,
                    error = finalError,
                    message = errorObj?.Message,
                    data = null
                };
            }
            catch (Exception deserializationEx)
            {
                Debug.Log("Deserialization error: " + deserializationEx.Message);
                return new GameResponse()
                {
                    data = null,
                    statusCode = (int)webRequest.responseCode,
                    error = webRequest.error
                };
            }
        }

        private void PrepareUrlWithParameters(RequestMessage request)
        {
            string finalUrl = request._requestPath;
            List<KeyValuePair<string, object>> parameters = request._requestParameters.ToList();
            for (int i = 0; i < parameters.Count; i++)
            {
                finalUrl += (i == 0 ? "?" : "&") + parameters[i].Key + "=" + parameters[i].Value;
            }
            request._requestPath = finalUrl;
        }
    }
}
