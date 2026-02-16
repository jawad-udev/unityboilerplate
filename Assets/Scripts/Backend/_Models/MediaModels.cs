using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Backend
{
    // ---------- Shared / Small DTOs ----------
    [Serializable]
    public class AgeGroup
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    [Serializable]
    public class ApiError
    {
        public string detail { get; set; }
    }

    // ---------- Media DTOs ----------
    [Serializable]
    public class MediaResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string media_type { get; set; }
        public long duration { get; set; }
        public long file_size { get; set; }
        public string owner_id { get; set; }
        public string owner_name { get; set; }
        public DateTime? uploaded_at { get; set; }
        public string status { get; set; }
        public string visibility { get; set; }
        public bool has_active_intervention { get; set; }
        public List<string> outcomes { get; set; }
        public List<AgeGroup> age_groups { get; set; }
        public string thumbnail_url { get; set; }
        public string media_url { get; set; }
        public object game_data_url { get; set; }
        public SpecializedData specialized_data { get; set; }
    }

    [Serializable]
    public class SpecializedData
    {
        public List<string> instruments { get; set; }
    }

    [Serializable]
    public class Meta
    {
        public int total_records { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public object next { get; set; }
        public object previous { get; set; }
    }

    [Serializable]
    public class MediaActionResponse
    {
        public string description { get; set; }
        public List<int> age_groups { get; set; }
        public List<string> outcomes { get; set; }
        public SpecializedData specialized_data { get; set; }
        public string visibility { get; set; }

        public string ToJson()
        {
            JObject jsonObject = new JObject();

            if (!string.IsNullOrEmpty(description))
                jsonObject["description"] = description;

            if (age_groups != null && age_groups.Count > 0)
                jsonObject["age_groups"] = JArray.FromObject(age_groups);

            if (outcomes != null && outcomes.Count > 0)
                jsonObject["outcomes"] = JArray.FromObject(outcomes);

            if (specialized_data != null &&
                specialized_data.instruments != null &&
                specialized_data.instruments.Count > 0)
            {
                // Use JObject.FromObject to properly nest the object instead of serializing to string
                jsonObject["specialized_data"] = JObject.FromObject(specialized_data);
            }

            if (!string.IsNullOrEmpty(visibility))
                jsonObject["visibility"] = visibility;

            return jsonObject.ToString(Formatting.None);
        }
    }

    [Serializable]
    public class PaginatedMediaResponse
    {
        public Meta meta { get; set; }
        public List<MediaResponse> results { get; set; }
    }

    [Serializable]
    public class RmtMediaItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string media_type { get; set; }
        public long duration { get; set; }
        public string owner_name { get; set; }
        public DateTime? uploaded_at { get; set; }
        public string media_url { get; set; }
        public string thumbnail_url { get; set; }
    }

    [Serializable]
    public class PaginatedRmtMediaResponse
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public List<RmtMediaItem> results { get; set; }
    }

    // Wrapper classes to distinguish different media types in AppService
    [Serializable]
    public class SearchMediaData
    {
        public List<MediaResponse> Items { get; set; }
    }

    [Serializable]
    public class RecentMediaData
    {
        public List<MediaResponse> Items { get; set; }
    }

    [Serializable]
    public class TrendingMediaData
    {
        public List<MediaResponse> Items { get; set; }
    }

    // Requests / Responses for Upload flow
    [Serializable]
    public class MediaUploadSessionRequest
    {
        public string media_file_name { get; set; }
        public string thumbnail_file_name { get; set; }
        public string media_content_type { get; set; }
        public string thumbnail_content_type { get; set; }
    }

    [System.Serializable]
    public class MediaUploadSessionResponse
    {
        public string upload_uuid;
        public MediaFile media;
        public MediaFile thumbnail;
    }

    [System.Serializable]
    public class MediaFile
    {
        public string s3_key;
        public string content_type;
        public string presigned_url;
    }

    [Serializable]
    public class MediaUploadConfirmRequest
    {
        public string media_s3_key { get; set; }
        public string thumbnail_s3_key { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string media_type { get; set; }
        public List<int> age_groups { get; set; }
        public List<string> outcomes { get; set; }
        public long duration { get; set; }
        public long file_size { get; set; }
        //public object media_file_metadata { get; set; }
        public Dictionary<string, object> specialized_data { get; set; } = new Dictionary<string, object>();
        public bool has_active_intervention { get; set; }
    }

    [Serializable]
    public class MediaUploadConfirmResponse
    {
        public string media_s3_key { get; set; }
        public string thumbnail_s3_key { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string media_type { get; set; }
        public List<int> age_groups { get; set; }
        public List<string> outcomes { get; set; }
        public long duration { get; set; }
        public long file_size { get; set; }
        public string media_file_metadata { get; set; }
        public string specialized_data { get; set; }
        public bool has_active_intervention { get; set; }
    }

    // Requests / Responses for Upload flow
    [Serializable]
    public class ProfileUploadSessionRequest
    {
        public string file_name { get; set; }
        public string content_type { get; set; }
    }

    [Serializable]
    public class ProfileUploadSessionResponse
    {
        public string upload_uuid { get; set; }
        public string s3_key { get; set; }
        public string presigned_url { get; set; }
        public string content_type { get; set; }
    }

    [Serializable]
    public class ProfileUploaConfirmRequest
    {
        public string s3_key { get; set; }
    }

    [Serializable]
    public class ProfileUploaConfirmResponse
    {
        public string profile_image_url { get; set; }
        public string status { get; set; }
    }

    [Serializable]
    public class CredentialUploadSessionRequest
    {
        public string primary_name;
        public string primary_content_type;
        public string secondary_name;
        public string secondary_content_type;

        public string ToJson()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            // Create a temporary dictionary to filter out empty strings
            var filtered = new
            {
                primary_name = string.IsNullOrEmpty(primary_name) ? null : primary_name,
                primary_content_type = string.IsNullOrEmpty(primary_content_type) ? null : primary_content_type,
                secondary_name = string.IsNullOrEmpty(secondary_name) ? null : secondary_name,
                secondary_content_type = string.IsNullOrEmpty(secondary_content_type) ? null : secondary_content_type
            };

            return JsonConvert.SerializeObject(filtered, settings);
        }
    }

    [Serializable]
    public class CredentialUploadSessionResponse
    {
        public string upload_uuid;
        public CredentialFileData primary;
        public CredentialFileData secondary;
        public string expires_at;
    }

    [Serializable]
    public class CredentialFileData
    {
        public string s3_key;
        public string presigned_url;
        public string content_type;
    }

    [Serializable]
    public class CredentialUploadConfirmRequest
    {
        public List<string> s3_keys;
    }

    [Serializable]
    public class CredentialUploadConfirmResponse
    {
        public string upload_uuid;
        public string status;
        public string message;
    }
}
