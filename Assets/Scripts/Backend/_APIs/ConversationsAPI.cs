// Model Classes
using System;
using System.Collections.Generic;
using Backend;
namespace Backend
{
    [Serializable]
    public class ConversationItem
    {
        public string id { get; set; }
        public OtherPerson other_person { get; set; }
        public int unread_count { get; set; }
        public DateTime last_message_at { get; set; }
    }

    [Serializable]
    public class OtherPerson
    {
        public int id { get; set; }
        public string full_name { get; set; }
        public string profile_image { get; set; }
        public string user_type { get; set; }
    }

    [Serializable]
    public class ConversationsResponse
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public List<ConversationItem> results { get; set; }
    }

    public class MessageData
    {
        public object next { get; set; }
        public object previous { get; set; }
        public List<Message> results { get; set; }
    }

    public class Message
    {
        public int id { get; set; }
        public int sender_id { get; set; }
        public string body { get; set; }
        public DateTime created_at { get; set; }
        public object read_at { get; set; }
        public bool is_me { get; set; }
    }

    public class SendMessageRequest
    {
        public string body { get; set; }
        public int recipient_id { get; set; }
    }

    public class SendMessageResponse
    {
        public string body { get; set; }
        public int recipient_id { get; set; }
    }

    // API Functions
    public static class ConversationsAPI
    {
        // Get conversations inbox
        public static void GetConversations(Action<ResponseMessage<List<ConversationItem>>> listener,
                                        int? limit = null,
                                        int? offset = null)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/conversations/";

            // Build query parameters
            List<string> queryParams = new List<string>();
            if (limit.HasValue)
                queryParams.Add($"limit={limit.Value}");
            if (offset.HasValue)
                queryParams.Add($"offset={offset.Value}");

            if (queryParams.Count > 0)
                requestPath += "?" + string.Join("&", queryParams);

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

        // Mark conversation as read
        public static void MarkConversationAsRead(string conversationId, Action<ResponseMessage<object>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/conversations/{conversationId}/mark-as-read/";

            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders
            };

            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);

            GameClient.Instance.DispatchRequest(req, listener);
        }

        // Get conversation messages/history
        public static void GetConversationMessages(string conversationId, Action<ResponseMessage<MessageData>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + $"api/v1/conversations/{conversationId}/messages/";

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

        // Send message
        public static void SendMessage(string messageBody, int recipientId, Action<ResponseMessage<SendMessageResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/conversations/send-message/";

            SendMessageRequest requestBody = new SendMessageRequest()
            {
                body = messageBody,
                recipient_id = recipientId
            };

            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(requestBody)
            };

            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);

            GameClient.Instance.DispatchRequest(req, listener);
        }
    }
}