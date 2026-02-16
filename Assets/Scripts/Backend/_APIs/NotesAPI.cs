using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public class NotesAPI
    {
        public static void GetNote(int noteId, Action<ResponseMessage<ClientNoteCreateResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/notes/{noteId}/";
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

        public static void CreateNote(int clientId, ClientNoteCreateRequest note, Action<ResponseMessage<ClientNoteCreateResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/notes/clients/{clientId}/notes/add/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(note)
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void UpdateNote(int noteId, ClientNoteCreateRequest note, Action<ResponseMessage<ClientNoteCreateResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/notes/{noteId}/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.PUT,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(note)
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void DeleteNote(int noteId, Action<ResponseMessage<object>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/notes/{noteId}/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.DELETE,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void GetClientNotes(int clientId, Action<ResponseMessage<List<Backend.ClientNotesResponse>>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/notes/clients/{clientId}/notes/";
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
    }
}
