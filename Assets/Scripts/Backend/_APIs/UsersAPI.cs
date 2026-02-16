using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public class UsersAPI
    {
        private static void AddCommonHeaders(RequestMessage req)
        {
            req._headers = RequestMessage._defaultHeaders;
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
        }

        public static void ChangePassword(string currentPassword, string newPassword, Action<ResponseMessage<object>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/users/change-password/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.PUT,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "current_password", currentPassword },
                    { "new_password", newPassword }
                })
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void GetUser(Action<ResponseMessage<object>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/users/get/";
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

        public static void UploadProfileImage(byte[] fileBytes, string fileName, Action<ResponseMessage<ProfileImageResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/users/upload-profile-image/";
            // Multipart handling assumed inside BabbleClient if content-type is set via headers
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.PUT,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "profile_image_file_name", fileName },
                    { "profile_image_bytes_base64", Convert.ToBase64String(fileBytes) }
                })
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void UploadProfileImageFile(string base64String, Action<ResponseMessage<ProfileImageResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/users/upload-profile-image/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.PUT,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "profile_image_file", base64String }
                })
            };

            AddCommonHeaders(req);
            GameClient.Instance.DispatchRequest(req, listener);
        }


        // POST /api/v1/media/uploads/session/
        public static void CreateUploadSession(ProfileUploadSessionRequest request, Action<ResponseMessage<ProfileUploadSessionResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/users/profile-image/presigned-url/";
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
        public static void ConfirmUpload(string uploadUuid, ProfileUploaConfirmRequest request, Action<ResponseMessage<ProfileUploaConfirmResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/users/profile-image/{uploadUuid}/confirm/";
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
    }
}
