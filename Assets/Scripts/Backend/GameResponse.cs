using System.Collections.Generic;
using Newtonsoft.Json;

namespace Backend
{
    public class GameResponse
    {
        public bool status;
        public int statusCode;
        public string message;
        public string error;
        public object data;
        public long _timeStamp;
        public string _payload;
        public RequestMessage _request;
        public Dictionary<string, object> _headers = new Dictionary<string, object>();
        public string access;
        public string refresh;

        public override string ToString()
        {
            return string.Format("[ResponseMessage] _statusCode = {0}, _entity = {1}, _message = {2}", statusCode, data, message);
        }
    }

    public class ErrorResponse
    {
        [JsonProperty("status")] public bool Status { get; set; }
        [JsonProperty("error")] public string Error { get; set; }
        [JsonProperty("message")] public string Message { get; set; }

        // Example: { "email": ["already exists"], "phone_number": ["already registered"] }
        [JsonProperty("data")] public Dictionary<string, string[]> Data { get; set; }
    }
}