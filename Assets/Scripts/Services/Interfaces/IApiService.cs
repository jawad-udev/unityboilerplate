using System;
using System.Collections.Generic;
using Backend;

public interface IApiService
{
    // RMT APIs
    void GetRecommendedRmts(Action<ResponseMessage<RecommendedRmtsResponse>> responseListener, bool? loadFromCache = null);
    void GetAvailableRmts(Action<ResponseMessage<AvailableRmtsResponse>> responseListener, bool? loadFromCache = null);

    // Auth APIs
    void UserLogin(string email, string password, Action<ResponseMessage<LoginResponse>> responseListener);
    void UserRegister(RegisterRmtRequest registerData, Action<ResponseMessage<RegisterRmtResponse>> responseListener);
    void RefreshToken(string refreshToken, Action<ResponseMessage<TokenRefreshResponse>> responseListener);
    void ForgotPassword(string email, Action<ResponseMessage<PasswordResetRequestResponse>> responseListener);
    void VerifyAccount(string email, string code, Action<ResponseMessage<AccountVerificationResponse>> responseListener);
    void VerifyCode(string email, Action<ResponseMessage<RequestVerificationCodeResponse>> responseListener);
    void Logout(string refreshToken, Action<ResponseMessage<LogoutResponse>> responseListener);
    void ResetPassword(string code, string email, string newPassword, Action<ResponseMessage<ResetPasswordResponse>> responseListener);
    void DeleteAccount(string refreshToken, Action<ResponseMessage<DeleteAccountResponse>> responseListener);
}
