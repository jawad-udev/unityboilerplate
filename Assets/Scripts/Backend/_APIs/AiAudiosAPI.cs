using System;
using System.Collections.Generic;
using UnityEngine;
using Backend;

// API wrapper
public static class AiAudiosAPI
{
    private static void AddCommonHeaders(RequestMessage req)
    {
        req._headers = RequestMessage._defaultHeaders;
        req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
        if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
            req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
    }

    public static void UploadUrl(string fileName, string contentType, Action<ResponseMessage<UploadUrlData>> listener)
    {
        string requestPath = GameClient.Instance._hostUrl + "api/v1/ai-audios/upload-url/";

        // Use a serializable DTO for JsonUtility
        var dto = new UploadUrlRequest()
        {
            file_name = fileName,
            content_type = contentType
        };

        RequestMessage req = new RequestMessage()
        {
            _requestType = RequestMessage.RequestType.POST,
            _payload = Guid.NewGuid().ToString(),
            _requestPath = requestPath,
            _body = GameSerializer.Serialize(dto)
        };

        AddCommonHeaders(req);
        GameClient.Instance.DispatchRequest(req, listener);
    }

    // GET /api/v1/ai-audios/jobs/{id}/
    public static void GetJobStatus(string jobId, Action<ResponseMessage<JobStatusData>> listener)
    {
        string requestPath = GameClient.Instance._hostUrl + $"api/v1/ai-audios/jobs/{jobId}/";

        RequestMessage req = new RequestMessage()
        {
            _requestType = RequestMessage.RequestType.GET,
            _payload = Guid.NewGuid().ToString(),
            _requestPath = requestPath
        };

        AddCommonHeaders(req);
        GameClient.Instance.DispatchRequest(req, listener);
    }
}
