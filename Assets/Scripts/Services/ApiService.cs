using System;
using System.Collections.Generic;
using Backend;
using UnityEngine;

public class ApiService : MonoBehaviour
{
    private bool IsSuccessStatus(int status)
    {
        return status >= 200 && status < 300;
    }

    #region Rmt APIs
    public void GetRecommendedRmts(Action<ResponseMessage<RecommendedRmtsResponse>> responseListener,
        bool? loadFromCache = null)
    {
        RMTsAPI.GetRecommendedRmts(responseListener, loadFromCache);
    }

    public void GetAvailableRmts(Action<ResponseMessage<AvailableRmtsResponse>> responseListener,
        bool? loadFromCache = null)
    {
        RMTsAPI.GetAvailableRmts(responseListener, loadFromCache);
    }
    #endregion

    #region Authentication APIs

    public void UserLogin(string email, string password, Action<ResponseMessage<LoginResponse>> responseListener)
    {
        AuthAPI.Login(email, password, responseListener);
    }

    public void UserRegister(RegisterRmtRequest registerData, Action<ResponseMessage<RegisterRmtResponse>> responseListener)
    {
        AuthAPI.Register(registerData, responseListener);
    }

    public void RefreshToken(string refreshToken, Action<ResponseMessage<TokenRefreshResponse>> responseListener)
    {
        AuthAPI.TokenRefresh(refreshToken, responseListener);
    }

    public void ForgotPassword(string email, Action<ResponseMessage<PasswordResetRequestResponse>> responseListener)
    {
        AuthAPI.ForgotPassword(email, responseListener);
    }

    public void VerifyAccount(string email, string code, Action<ResponseMessage<AccountVerificationResponse>> responseListener)
    {
        AuthAPI.VerifyAccount(email, code, responseListener);
    }

    public void VerifyCode(string email, Action<ResponseMessage<RequestVerificationCodeResponse>> responseListener)
    {
        AuthAPI.VerifyCode(email, responseListener);
    }

    public void Logout(string refreshToken, Action<ResponseMessage<LogoutResponse>> responseListener)
    {
        AuthAPI.Logout(refreshToken, responseListener);
    }

    public void ResetPassword(string code, string email, string newPassword, Action<ResponseMessage<ResetPasswordResponse>> responseListener)
    {
        AuthAPI.ResetPassword(code, email, newPassword, responseListener);
    }

    public void DeleteAccount(string refreshToken, Action<ResponseMessage<DeleteAccountResponse>> responseListener)
    {
        AuthAPI.DeleteAccount(refreshToken, responseListener);
    }

    #endregion

    #region Playlist APIs

    public void GetPlaylists(Action<ResponseMessage<List<PlaylistSummaryResponse>>> responseListener,
        bool? loadFromCache = null)
    {
        PlaylistAPI.GetPlaylists(responseListener, loadFromCache);
    }

    public void CreatePlaylist(PlaylistCreateRequest request, Action<ResponseMessage<PlaylistCreateResponse>> responseListener)
    {
        PlaylistAPI.CreatePlaylist(request, responseListener);
    }

    public void GetPlaylist(string id, Action<ResponseMessage<PlaylistDetailResponse>> responseListener)
    {
        PlaylistAPI.GetPlaylist(id, responseListener);
    }

    public void GetUserPlaylist(Action<ResponseMessage<List<PlaylistSummaryResponse>>> responseListener)
    {
        PlaylistAPI.GetUserPlaylists(responseListener);
    }

    public void UpdatePlaylist(string id, PlaylistUpdateRequest request, Action<ResponseMessage<PlaylistDetailResponse>> responseListener)
    {
        PlaylistAPI.UpdatePlaylist(id, request, responseListener);
    }

    public void PatchPlaylist(string id, Dictionary<string, object> patchBody, Action<ResponseMessage<PlaylistDetailResponse>> responseListener)
    {
        PlaylistAPI.PatchPlaylist(id, patchBody, responseListener);
    }

    public void DeletePlaylist(string id, Action<ResponseMessage<object>> responseListener)
    {
        PlaylistAPI.DeletePlaylist(id, responseListener);
    }

    public void AddMediaToPlaylist(string playlistId, PlaylistMediaModifyRequest request, Action<ResponseMessage<List<PlaylistMediaModifyResponse>>> responseListener)
    {
        PlaylistAPI.AddMediaToPlaylist(playlistId, request, responseListener);
    }

    public void RemoveMediaFromPlaylist(string playlistId, PlaylistMediaModifyRequest request, Action<ResponseMessage<PlaylistMediaModifyResponse>> responseListener)
    {
        PlaylistAPI.RemoveMediaFromPlaylist(playlistId, request, responseListener);
    }

    public void ReorderPlaylistMedia(string playlistId, PlaylistReorderRequest request, Action<ResponseMessage<PlaylistMediaReorderResponse>> responseListener)
    {
        PlaylistAPI.ReorderPlaylistMedia(playlistId, request, responseListener);
    }

    public void CreatePlaylistUploadSession(PlaylistUploadSessionRequest request, Action<PlaylistUploadSessionResponse> onSuccess, Action<string> onError)
    {
        PlaylistAPI.CreatePlaylistUploadSession(request, (response) =>
        {
            if (response._statusCode >= 200 && response._statusCode < 300 && response._entity != null)
            {
                onSuccess?.Invoke(response._entity);
            }
            else
            {
                onError?.Invoke(response._error ?? "Upload session creation failed");
            }
        });
    }

    #endregion

    #region Media APIs

    public void GetMedia(string mediaId, Action<ResponseMessage<MediaResponse>> responseListener,
        bool? loadFromCache = null)
    {
        MediaAPI.GetMedia(mediaId, responseListener, loadFromCache);
    }

    public void SearchMedia(
    int? ageGroup = null,
    int? limit = null,
    int? offset = null,
    string outcomesCsv = null,
    int? owner = null,
    string q = null,
    string mediaTypeCsv = null,
    bool? hasActiveIntervention = null,
    Action<ResponseMessage<PaginatedMediaResponse>> responseListener = null,
    bool? loadFromCache = null)
    {
        MediaAPI.SearchMedia(
            ageGroup: ageGroup,
            limit: limit,
            offset: offset,
            outcomesCsv: outcomesCsv,
            owner: owner,
            q: q,
            mediaTypeCsv: mediaTypeCsv,
            hasActiveIntervention: hasActiveIntervention,
            listener: responseListener,
            loadFromCache: loadFromCache);
    }

    public void GetMyRecentMedia(int limit, int offset, Action<ResponseMessage<List<MediaResponse>>> responseListener,
        bool? loadFromCache = null)
    {
        MediaAPI.GetMyRecentMedia(limit, offset, responseListener, loadFromCache);
    }

    public void GetTrendingMedia(int limit, int offset, Action<ResponseMessage<List<MediaResponse>>> responseListener,
        bool? loadFromCache = null)
    {
        MediaAPI.GetTrendingMedia(limit, offset, responseListener, loadFromCache);
    }

    public void GetMediaByAgeGroup(int ageGroupId, int limit, int offset, Action<ResponseMessage<PaginatedMediaResponse>> responseListener,
        bool? loadFromCache = null)
    {
        MediaAPI.GetMediaByAgeGroup(ageGroupId, limit, offset, responseListener, loadFromCache);
    }

    public void GetMediaByOutcome(string outcomesCsv, int limit, int offset, Action<ResponseMessage<PaginatedMediaResponse>> responseListener,
        bool? loadFromCache = null)
    {
        MediaAPI.GetMediaByOutcome(outcomesCsv, limit, offset, responseListener, loadFromCache);
    }

    public void GetPublicMedia(int limit, int offset, Action<ResponseMessage<PaginatedMediaResponse>> responseListener,
        bool? loadFromCache = null)
    {
        MediaAPI.GetPublicMedia(limit, offset, responseListener, loadFromCache);
    }

    public void GetRecentMedia(int limit, int offset, Action<ResponseMessage<PaginatedMediaResponse>> responseListener,
        bool? loadFromCache = null)
    {
        MediaAPI.GetRecentMedia(limit, offset, responseListener, loadFromCache);
    }

    public void GetMyMedia(int limit, int offset, Action<ResponseMessage<PaginatedMediaResponse>> responseListener,
        bool? loadFromCache = null)
    {
        MediaAPI.GetMyMedia(limit, offset, responseListener, loadFromCache);
    }

    public void GetRmtMedia(int rmtId, string mediaType, int limit, int offset, Action<ResponseMessage<PaginatedRmtMediaResponse>> responseListener,
        bool? loadFromCache = null)
    {
        MediaAPI.GetRmtMedia(rmtId, mediaType, limit, offset, responseListener, loadFromCache);
    }

    public void UpdateMediaPut(string id, MediaActionResponse payload, Action<ResponseMessage<MediaActionResponse>> responseListener)
    {
        MediaAPI.UpdateMediaPut(id, payload, responseListener);
    }

    public void UpdateMediaPatch(string id, Dictionary<string, object> patchBody, Action<ResponseMessage<MediaActionResponse>> responseListener)
    {
        MediaAPI.UpdateMediaPatch(id, patchBody, responseListener);
    }

    public void DeleteMediaAction(string id, Action<ResponseMessage<object>> responseListener)
    {
        MediaAPI.DeleteMediaAction(id, responseListener);
    }

    public void SaveToggleMedia(string mediaId, Action<ResponseMessage<object>> responseListener)
    {
        MediaAPI.SaveToggle(mediaId, responseListener);
    }

    // public void LikeToggleMedia(string mediaId, Action<ResponseMessage<object>> responseListener)
    // {
    //     MediaAPI.LikeToggle(mediaId, (response) =>
    //     {
    //         if (response._status)
    //         {
    //             // Invalidate liked media cache and re-fetch
    //             CachePolicy.InvalidateLikedMedia();
    //             Services.UserService.PrefetchLikedMedia();
    //         }
    //         responseListener?.Invoke(response);
    //     });
    // }

    public void GetLikedMedia(int limit, int offset, string mediaTypeCsv, Action<ResponseMessage<PaginatedMediaResponse>> responseListener)
    {
        MediaAPI.GetLikedMedia(limit, offset, mediaTypeCsv, responseListener);
    }

    #endregion

    #region RMT (Registered Music Therapist) APIs

    public void GetRmts(Action<ResponseMessage<RmtUser>> responseListener,
        bool? loadFromCache = null)
    {
        RMTsAPI.GetRmts(responseListener, loadFromCache);
    }

    public void GetRmtsDataLists(Action<ResponseMessage<RmtDataListsResponse>> responseListener,
        bool? loadFromCache = null)
    {
        RMTsAPI.GetRmtsDataLists(responseListener, loadFromCache);
    }

    public void UpdateRmtProfile(UpdateRMTProfileRequest request, Action<ResponseMessage<UpdateRMTProfileResponse>> responseListener)
    {
        RMTsAPI.UpdateRmtProfile(request, responseListener);
    }

    public void CreateRmtUploadSession(CredentialUploadSessionRequest request, Action<ResponseMessage<CredentialUploadSessionResponse>> responseListener)
    {
        RMTsAPI.CreateUploadSession(request, responseListener);
    }

    public void ConfirmRmtUpload(string uploadUuid, CredentialUploadConfirmRequest request, Action<ResponseMessage<CredentialUploadConfirmResponse>> responseListener)
    {
        RMTsAPI.ConfirmUpload(uploadUuid, request, responseListener);
    }

    // public void SubscribeToRmt(int rmtId, Action<ResponseMessage<RmtSubscriptionResponse>> responseListener)
    // {
    //     RMTsAPI.SubscribeToRmt(rmtId, (response) =>
    //     {
    //         if (IsSuccessStatus(response._statusCode))
    //         {
    //             // Invalidate subscribed RMTs cache and re-fetch
    //             CachePolicy.InvalidateSubscribedRmts();
    //             Services.UserService.PrefetchSubscribedRmts();
    //         }
    //         responseListener?.Invoke(response);
    //     });
    // }

    // public void UnsubscribeFromRmt(int rmtId, Action<ResponseMessage<object>> responseListener)
    // {
    //     RMTsAPI.UnsubscribeFromRmt(rmtId, (response) =>
    //     {
    //         if (IsSuccessStatus(response._statusCode))
    //         {
    //             // Invalidate subscribed RMTs cache and re-fetch
    //             CachePolicy.InvalidateSubscribedRmts();
    //             Services.UserService.PrefetchSubscribedRmts();
    //         }
    //         responseListener?.Invoke(response);
    //     });
    // }

    public void GetRmtDetails(int rmtId, Action<ResponseMessage<RmtDetailResponse>> responseListener,
        bool? loadFromCache = null)
    {
        RMTsAPI.GetRmtDetails(rmtId, responseListener, loadFromCache);
    }

    #endregion

    #region Client Management APIs

    public void AddOfflineClient(string fullName, string email, Action<ResponseMessage<AddOfflineClientResponse>> responseListener)
    {
        ClientsAPI.AddOfflineClient(fullName, email, responseListener);
    }

    public void GetClients(Action<ResponseMessage<List<RmtClientResponse>>> responseListener)
    {
        ClientsAPI.GetClients(responseListener);
    }

    public void RedeemInvitation(string token, int clientId, Action<ResponseMessage<InvitationRedeemResponse>> responseListener)
    {
        ClientsAPI.RedeemInvitation(token, clientId, responseListener);
    }

    #endregion

    #region Notes APIs

    public void GetNote(int noteId, Action<ResponseMessage<ClientNoteCreateResponse>> responseListener)
    {
        NotesAPI.GetNote(noteId, responseListener);
    }

    public void CreateNote(int clientId, ClientNoteCreateRequest note, Action<ResponseMessage<ClientNoteCreateResponse>> responseListener)
    {
        NotesAPI.CreateNote(clientId, note, responseListener);
    }

    public void UpdateNote(int noteId, ClientNoteCreateRequest note, Action<ResponseMessage<ClientNoteCreateResponse>> responseListener)
    {
        NotesAPI.UpdateNote(noteId, note, responseListener);
    }

    public void DeleteNote(int noteId, Action<ResponseMessage<object>> responseListener)
    {
        NotesAPI.DeleteNote(noteId, responseListener);
    }

    public void GetClientNotes(int clientId, Action<ResponseMessage<List<ClientNotesResponse>>> responseListener)
    {
        NotesAPI.GetClientNotes(clientId, responseListener);
    }

    #endregion

    #region Consumer APIs

    public void GetConsumers(Action<ResponseMessage<UserInfo>> responseListener)
    {
        ConsumersAPI.GetConsumers(responseListener);
    }

    public void UpdateConsumerProfile(UpdateConsumerProfileRequest request, Action<ResponseMessage<UpdateConsumerProfileResponse>> responseListener)
    {
        ConsumersAPI.UpdateConsumerProfile(request, responseListener);
    }

    public void PatchConsumerProfile(Dictionary<string, object> patchData, Action<ResponseMessage<UpdateConsumerProfileResponse>> responseListener)
    {
        ConsumersAPI.PatchConsumerProfile(patchData, responseListener);
    }

    public void UploadProfileImage(string base64String, Action<ResponseMessage<ProfileImageResponse>> responseListener)
    {
        UsersAPI.UploadProfileImageFile(base64String, responseListener);
    }

    public void UploadProfilePhotoSession(ProfileUploadSessionRequest request, Action<ProfileUploadSessionResponse> onSuccess, Action<string> onError)
    {
        Backend.UsersAPI.CreateUploadSession(request, (response) =>
        {
            if (response._status && response._entity != null)
                onSuccess?.Invoke(response._entity);
            else
                onError?.Invoke(response._error ?? "Upload profile image failed");
        });
    }

    public void ConfirmProfilePhotoUpload(string uploadUuid, ProfileUploaConfirmRequest request, Action<Backend.ProfileUploaConfirmResponse> onSuccess, Action<string> onError)
    {
        Backend.UsersAPI.ConfirmUpload(uploadUuid, request, (response) =>
        {
            if (response._status && response._entity != null)
                onSuccess?.Invoke(response._entity);
            else
                onError?.Invoke(response._error ?? "Upload profile image failed");
        });
    }

    // GET handler
    public void GetSettings(Action<ResponseMessage<UserSettingsData>> responseListener,
        bool? loadFromCache = null)
    {
        ConsumersAPI.GetSettings(responseListener, loadFromCache);
    }

    // PUT handler
    public void UpdateSettings(UserSettingsData settingsData, Action<ResponseMessage<UserSettingsData>> responseListener)
    {
        ConsumersAPI.UpdateSettings(settingsData, responseListener);
    }

    // GET connected RMTs handler (paginated)
    public void GetConnectedRmts(int limit, int offset, Action<ResponseMessage<PaginatedConnectedRmtsResponse>> responseListener,
        bool? loadFromCache = null)
    {
        ConsumersAPI.GetConnectedRmts(limit, offset, responseListener, loadFromCache);
    }

    // GET my subscribed RMTs handler (paginated)
    public void GetMySubscribedRmts(int limit, int offset, Action<ResponseMessage<PaginatedConnectedRmtsResponse>> responseListener,
        bool? loadFromCache = null)
    {
        RMTsAPI.GetMySubscribedRmts(limit, offset, responseListener, loadFromCache);
    }

    #endregion

    #region Appointments APIs

    public void BookAppointment(ConsultationBookingRequest booking, Action<ResponseMessage<ConsultationBookingResponse>> responseListener)
    {
        AppointmentsAPI.BookAppointment(booking, responseListener);
    }

    public void GetUpcomingAppointments(int limit, int offset, Action<ResponseMessage<List<AppointmentItem>>> responseListener,
        bool? loadFromCache = null)
    {
        AppointmentsAPI.GetUpcomingAppointments(limit, offset, responseListener, loadFromCache);
    }

    public void GetConsumerAppointments(int limit, int offset, Action<ResponseMessage<PaginatedAppointmentsResponse>> responseListener,
        bool? loadFromCache = null)
    {
        AppointmentsAPI.GetConsumerAppointments(limit, offset, responseListener, loadFromCache);
    }

    public void GetAppointmentAvailability(int rmtId, float duration_minutes, Action<ResponseMessage<AppointmentAvailabilityResponse>> responseListener,
        bool? loadFromCache = null)
    {
        AppointmentsAPI.GetAppointmentAvailability(rmtId, duration_minutes, responseListener, loadFromCache);
    }

    #endregion

    #region AI Audio APIs

    public void GetUploadUrl(string fileName, string contentType, Action<ResponseMessage<UploadUrlData>> responseListener)
    {
        AiAudiosAPI.UploadUrl(fileName, contentType, responseListener);
    }

    public void GetJobStatus(string jobId, Action<ResponseMessage<JobStatusData>> responseListener)
    {
        AiAudiosAPI.GetJobStatus(jobId, responseListener);
    }

    #endregion

    #region Health Check APIs

    public void GetHealthDetailed(Action<ResponseMessage<object>> responseListener)
    {
        HealthAPI.GetHealthDetailed(responseListener);
    }


    #endregion

    #region ConversationsAPI
    // Get conversations inbox
    public void GetConversations(Action<List<ConversationItem>> onSuccess, Action<string> onError,
                                int? limit = null, int? offset = null)
    {
        Backend.ConversationsAPI.GetConversations((response) =>
        {
            if (IsSuccessStatus(response._statusCode))
                onSuccess?.Invoke(response._entity);
            else
                onError?.Invoke(response._error ?? "Failed to get conversations");
        }, limit, offset);
    }

    // Mark conversation as read
    public void MarkConversationAsRead(string conversationId, Action onSuccess, Action<string> onError)
    {
        Backend.ConversationsAPI.MarkConversationAsRead(conversationId, (response) =>
        {
            if (IsSuccessStatus(response._statusCode))
                onSuccess?.Invoke();
            else
                onError?.Invoke(response._error ?? "Failed to mark conversation as read");
        });
    }

    // Get conversation messages
    public void GetConversationMessages(string conversationId, Action<MessageData> onSuccess, Action<string> onError)
    {
        Backend.ConversationsAPI.GetConversationMessages(conversationId, (response) =>
        {
            if (IsSuccessStatus(response._statusCode))
                onSuccess?.Invoke(response._entity);
            else
                onError?.Invoke(response._error ?? "Failed to get conversation messages");
        });
    }

    // Send message
    public void SendMessage(string messageBody, int recipientId, Action<SendMessageResponse> onSuccess, Action<string> onError)
    {
        Backend.ConversationsAPI.SendMessage(messageBody, recipientId, (response) =>
        {
            if (IsSuccessStatus(response._statusCode))
                onSuccess?.Invoke(response._entity);
            else
                onError?.Invoke(response._error ?? "Failed to send message");
        });
    }
    #endregion
}
