using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public class AuthAPI
    {
        // X-APP-Token header will be added by caller via RequestMessage._headers or BabbleClient defaults
        public static void Login(string email, string password, Action<ResponseMessage<LoginResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/auth/login/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "email", email },
                    { "password", password },
                    {"user_type", "CONSUMER"}
                })
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void Register(RegisterRmtRequest registerData, Action<ResponseMessage<RegisterRmtResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/auth/register/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(registerData)
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void TokenRefresh(string refreshToken, Action<ResponseMessage<TokenRefreshResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/auth/token/refresh/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "refresh", refreshToken }
                })
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void ForgotPassword(string email, Action<ResponseMessage<PasswordResetRequestResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/auth/forgot-password/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "email", email }
                })
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void VerifyAccount(string email, string code, Action<ResponseMessage<AccountVerificationResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/auth/verify-account/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "email", email },
                    { "code", code }
                })
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void VerifyCode(string email, Action<ResponseMessage<RequestVerificationCodeResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/auth/account/verify-code/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "email", email },
                    //{ "code", code }
                })
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void DeleteAccount(string refreshToken, Action<ResponseMessage<DeleteAccountResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/auth/delete/account/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "refresh_token", refreshToken }
                })
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            // Add Authorization if available
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void Logout(string refreshToken, Action<ResponseMessage<LogoutResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/auth/logout/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "refresh", refreshToken }
                })
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void ResetPassword(string code, string email, string newPassword, Action<ResponseMessage<ResetPasswordResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/auth/reset-password/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(new Dictionary<string, object>() {
                    { "code", code },
                    { "email", email },
                    { "password", newPassword }
                })
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }
    }
}
