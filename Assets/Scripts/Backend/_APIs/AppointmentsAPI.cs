using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    public class AppointmentsAPI
    {
        public static void BookAppointment(ConsultationBookingRequest booking, Action<ResponseMessage<ConsultationBookingResponse>> listener)
        {
            string requestPath = GameClient.Instance._hostUrl + "api/v1/appointments/booking/";
            RequestMessage req = new RequestMessage()
            {
                _requestType = RequestMessage.RequestType.POST,
                _payload = Guid.NewGuid().ToString(),
                _requestPath = requestPath,
                _headers = RequestMessage._defaultHeaders,
                _body = GameSerializer.Serialize(booking)
            };
            req._headers.Add("X-APP-Token", GameClient.Instance.AppToken);
            if (!string.IsNullOrEmpty(GameClient.Instance.AccessToken))
                req._headers.Add("Authorization", "Bearer " + GameClient.Instance.AccessToken);
            GameClient.Instance.DispatchRequest(req, listener);
        }

        public static void GetUpcomingAppointments(int limit, int offset, Action<ResponseMessage<List<AppointmentItem>>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetUpcomingAppointments;
            string requestPath = $"{GameClient.Instance._hostUrl}api/v1/appointments/list/?limit={limit}&offset={offset}";

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

            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        public static void GetConsumerAppointments(int limit, int offset, Action<ResponseMessage<PaginatedAppointmentsResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetConsumerAppointments;
            string requestPath = $"{GameClient.Instance._hostUrl}api/v1/appointments/consumer/list/?limit={limit}&offset={offset}";

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

            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }

        // GET /api/v1/appointments/availability/{rmt_id}/?duration={duration_minutes}
        public static void GetAppointmentAvailability(int rmtId, float duration_minutes, Action<ResponseMessage<AppointmentAvailabilityResponse>> listener,
            bool? loadFromCache = null)
        {
            var policy = CachePolicy.GetAppointmentAvailability;
            string requestPath = $"{GameClient.Instance._hostUrl}api/v1/appointments/availability/{rmtId}/?duration={duration_minutes}";

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

            GameClient.Instance.DispatchRequest(req, listener,
                loadFromCache ?? policy.shouldCache,
                policy.ttlSeconds,
                policy.cacheType);
        }
    }
}
