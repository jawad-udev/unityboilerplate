using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Backend
{
    [System.Serializable]
    public class CachedResponse
    {
        public string entityJson;

        public bool status;
        public string message;
        public string error;
        public string payload;
        public int statusCode;
        public string access;
        public string refresh;

        public string requestJson;  // to restore RequestMessage if needed
    }
    
    [Serializable]
    public class LoginResponse
    {
        public User user;
        public string access;
        public string refresh;
        public bool force_update;
        public bool is_approved;
        public string rmt_status;
    }

    [Serializable]
    public class User
    {
        public int id;
        public string first_name;
        public string last_name;
        public string email;
        public string password;
        public string profile_image_url;
    }

    [Serializable]
    public class RegisterRmtRequest
    {
        public string first_name;
        public string last_name;
        public string email;
        public string password;
        public string user_type;
        public string org_name;
        public string org_country;
        public string org_registration_number;
        public string location;
        public string phone_number;
    }

    [Serializable]
    public class RegisterRmtResponse
    {
        public int id;
        public string first_name;
        public string last_name;
        public string email;
        public string user_type;
        // swagger included these in the 201 response — kept for completeness
        public string location;
        public string phone_number;
    }

    [Serializable]
    public class TokenRefreshResponse
    {
        public string access;
        public string refresh;
    }

    [Serializable]
    public class PasswordResetRequestResponse
    {
        // swagger forgot-password returns email; some endpoints include expiry_minutes — keep both
        public string email;
        public int expiry_minutes;
    }

    [Serializable]
    public class AccountVerificationResponse
    {
        public string email;
        public string code;
    }

    [Serializable]
    public class RequestVerificationCodeResponse
    {
        public string email;
    }

    [Serializable]
    public class DeleteAccountResponse
    {
        public string refresh_token;
    }

    [Serializable]
    public class LogoutResponse
    {
        public string refresh;
    }

    [Serializable]
    public class ResetPasswordResponse
    {
        // swagger shows email, code, password — include all
        public string email;
        public string code;
        public string password;
    }

    // Appointments
    [Serializable]
    public class ConsultationBookingRequest
    {
        public string title;
        public string description;
        public string start_time; // ISO datetime string
        public string end_time;   // ISO datetime string
        public int invitee_id;
        public string timezone;
    }

    [Serializable]
    public class ConsultationBookingResponse
    {
        public int id;
        public string title;
        public string description;
        public string start_time;
        public string end_time;
        public int rmt;
        public int consumer;
        public string host_role;
        public string event_id;
    }

    [Serializable]
    public class AppointmentItem
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public int duration_minutes { get; set; }
        public string status { get; set; }
        public bool is_pre_booked { get; set; }
        public string host_role { get; set; }
        public string provider { get; set; }
        public string event_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int rmt { get; set; }
        public int rmt_client { get; set; }
        public object consumer { get; set; }
        public int client_id { get; set; }
        public string invitee_name { get; set; }
        public string invitee_email { get; set; }
    }

    [Serializable]
    public class PaginatedAppointmentsResponse
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public List<AppointmentItem> results { get; set; }
    }

    // Clients
    [Serializable]
    public class AddOfflineClientResponse
    {
        public int id;
        public string full_name;
        public string rmt; // swagger shows email string
        public bool is_registered;
    }

    [Serializable]
    public class RmtClientResponse
    {
        public int id;
        public string full_name;
        public bool is_registered;
    }

    [Serializable]
    public class InvitationRedeemResponse
    {
        public string token;
        public int client_id;
    }

    // Notes
    [Serializable]
    public class ClientNoteCreateRequest
    {
        public string title;
        public List<string> goals;
        public string outcome;
        public string progress_summary;
        public string additional_notes;
    }

    [Serializable]
    public class ClientNoteCreateResponse
    {
        public int id;
        public string title;
        public List<string> goals;
        public string outcome;
        public string progress_summary;
        public string additional_notes;
        public DateTime created_at;
        public DateTime updated_at;
    }

    [Serializable]
    public class ClientNotesResponse
    {
        public int id { get; set; }
        public string title { get; set; }
        public List<string> goals { get; set; }
        public string outcome { get; set; }
        public string progress_summary { get; set; }
        public DateTime created_at { get; set; }
    }

    [Serializable]
    public class ClientNoteListResponse
    {
        public List<ClientNoteCreateResponse> notes;
    }

    // Consumers
    [Serializable]
    public class UpdateConsumerProfileRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? age_at_signup;
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string gender;
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string first_name;
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string last_name;
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string app_users;
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<int> mind_emotions;
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<int> cognitive_support;
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<int> health_recovery;
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<int> lifestyle;
    }

    [Serializable]
    public class UpdateConsumerProfileResponse
    {
        // Also include top-level fields to match swagger that returns the object directly:
        public int age_at_signup;
        public string gender;
        public string first_name;
        public string last_name;
        public string app_users;
        public List<int> mind_emotions;
        public List<int> cognitive_support;
        public List<int> health_recovery;
        public List<int> lifestyle;
    }

    [Serializable]
    public class ConsumerProfile
    {
        public List<string> mind_emotions;
        public List<string> cognitive_support;
        public List<string> health_recovery;
        public List<string> lifestyle;
        public string app_users;
    }

    [Serializable]
    public class UserInfo
    {
        public int id;
        public string first_name;
        public string last_name;
        public string email;
        public string location;
        public string phone_number;
        public string gender;
        public int age_at_signup;
        public string profile_image_url;
        public ConsumerProfile consumer_profile;

        // Optional: Constructor for easy initialization
        public UserInfo()
        {
            consumer_profile = new ConsumerProfile();
        }
    }

    [Serializable]
    public class LookupItem
    {
        public int id;
        public string name;
        public string slug;
        public IDictionary<string, object> extra;
    }

    [Serializable]
    public class RmtDataListsResponse
    {
        public List<LookupItem> expertise;
        public List<LookupItem> techniques;
        public List<LookupItem> music_styles;
        public List<LookupItem> client_groups;
        public IDictionary<string, object> additionalData;
    }

    // RMTs
    [Serializable]
    public class UpdateRMTProfileRequest
    {
        public string location;
        public string phone_number;
        public List<int> areas_expertise;
        public List<int> therapeutic_techniques;
        public List<int> music_styles;
        public List<int> client_groups;
        public object other_expertise;
        public object other_techniques;
        public object other_music_styles;
        public string org_name;
        public string org_country;
        public string org_registration_number;


        public string ToJson()
        {
            JObject jsonObject = new JObject();

            if (!string.IsNullOrEmpty(location))
                jsonObject["location"] = location;

            if (!string.IsNullOrEmpty(phone_number))
                jsonObject["phone_number"] = phone_number;

            if (areas_expertise != null && areas_expertise.Count > 0)
                jsonObject["areas_expertise"] = JArray.FromObject(areas_expertise);

            if (therapeutic_techniques != null && therapeutic_techniques.Count > 0)
                jsonObject["therapeutic_techniques"] = JArray.FromObject(therapeutic_techniques);

            if (music_styles != null && music_styles.Count > 0)
                jsonObject["music_styles"] = JArray.FromObject(music_styles);

            if (client_groups != null && client_groups.Count > 0)
                jsonObject["client_groups"] = JArray.FromObject(client_groups);

            if (other_expertise != null)
                jsonObject["other_expertise"] = JToken.FromObject(other_expertise);

            if (other_techniques != null)
                jsonObject["other_techniques"] = JToken.FromObject(other_techniques);

            if (other_music_styles != null)
                jsonObject["other_music_styles"] = JToken.FromObject(other_music_styles);

            if (!string.IsNullOrEmpty(org_name))
                jsonObject["org_name"] = org_name;

            if (!string.IsNullOrEmpty(org_country))
                jsonObject["org_country"] = org_country;

            if (!string.IsNullOrEmpty(org_registration_number))
                jsonObject["org_registration_number"] = org_registration_number;

            return jsonObject.ToString(Formatting.None);
        }
    }

    [Serializable]
    public class UpdateRMTProfileResponse
    {
        public List<int> areas_expertise;
        public List<int> therapeutic_techniques;
        public List<int> music_styles;
        public List<int> client_groups;
        public List<string> other_expertise;
        public List<string> other_techniques;
        public List<string> other_music_styles;
    }

    [Serializable]
    public class RmtUser
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string location { get; set; }
        public string phone_number { get; set; }
        public string profile_image_url { get; set; }
        public RmtProfile rmt_profile { get; set; }
    }

    [Serializable]
    public class RecommendedRmt
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string profile_image_url { get; set; }
    }

    [Serializable]
    public class AvailableRmt : RecommendedRmt
    {
    }

    [Serializable]
    public class RecommendedRmtsResponse : List<RecommendedRmt>
    {
    }

    [Serializable]
    public class AvailableRmtsResponse : List<AvailableRmt>
    {
    }

    [Serializable]
    public class Organization
    {
        public int id { get; set; }
        public string name { get; set; }
        public string country { get; set; }
    }

    [Serializable]
    public class RmtProfile
    {
        public string title { get; set; }
        public string intro { get; set; }
        public string org_registration_number { get; set; }
        public Organization organization { get; set; }
        public List<string> areas_expertise { get; set; }
        public List<string> therapeutic_techniques { get; set; }
        public List<string> music_styles { get; set; }
        public List<string> client_groups { get; set; }
        public List<string> other_expertise { get; set; }
        public List<string> other_techniques { get; set; }
        public List<string> other_music_styles { get; set; }
        public int subscribers_count { get; set; }
        public int total_streams_count { get; set; }
        public int total_uploads_count { get; set; }
        public int total_likes_count { get; set; }
    }

    [Serializable]
    public class RmtDetailResponse
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string location { get; set; }
        public string phone_number { get; set; }
        public string profile_image_url { get; set; }
        public RmtProfile rmt_profile { get; set; }
        public int media_count { get; set; }
        public List<MediaResponse> best_music { get; set; }
        public List<MediaResponse> best_video { get; set; }
    }

    [Serializable]
    public class ConnectedRmtSummary
    {
        public int id { get; set; }
        public string display_name { get; set; }
        public int subscribers_count { get; set; }
        public string profile_image_url { get; set; }
    }

    [Serializable]
    public class PaginatedConnectedRmtsResponse
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public List<ConnectedRmtSummary> results { get; set; }
    }

    // Users
    [Serializable]
    public class ProfileImageResponse
    {
        public string profile_image_url;
    }

    // Health-detailed (swagger returns dynamic keys additionalProp1/2/3)
    [Serializable]
    public class HealthDetailedResponse
    {
        // Use a dictionary to hold dynamic health props
        public Dictionary<string, string> additionalProp;
    }

    [Serializable]
    public class UserSettingsData
    {
        public MediaQuality media_quality;
        public Notifications notifications;
        public Preferences preferences;
    }

    [Serializable]
    public class MediaQuality
    {
        public string wifi;
        public string cellular;
    }

    [Serializable]
    public class Notifications
    {
        public NotificationType audio;
        public NotificationType video;
        public NotificationType podcast;
    }

    [Serializable]
    public class NotificationType
    {
        public bool app;
        public bool email;
    }

    [Serializable]
    public class Preferences
    {
        public bool select_profile_on_startup;
    }

    [Serializable]
    public class RmtSubscriptionResponse
    {
        public int id;
        public int consumer;
        public int rmt;
        public string created_at;
    }

    [Serializable]
    public class RmtNavigationData
    {
        public int RmtId { get; set; }
        public RmtDetailResponse RmtDetails { get; set; }

        public RmtNavigationData()
        {
        }

        public RmtNavigationData(int rmtId)
        {
            RmtId = rmtId;
        }

        public RmtNavigationData(RmtDetailResponse rmtDetails)
        {
            if (rmtDetails != null)
            {
                RmtId = rmtDetails.id;
                RmtDetails = rmtDetails;
            }
        }
    }

    // Appointment Availability
    [Serializable]
    public class TimeSlot
    {
        public string start_time { get; set; }
        public string end_time { get; set; }
        public bool is_available { get; set; }
    }

    [Serializable]
    public class AvailabilityDay
    {
        public string date { get; set; }
        public string display { get; set; }
        public int day_of_week { get; set; }
        public List<TimeSlot> slots { get; set; }
    }

    [Serializable]
    public class AppointmentAvailabilityResponse : List<AvailabilityDay>
    {
    }

    // UI Media Play Button Structure
    [Serializable]
    public class MediaPlayButton
    {
        public UnityEngine.UI.Button button;
        public UnityEngine.UI.Image thumbnail;
        public UnityEngine.UI.Image icon;
    }

}
