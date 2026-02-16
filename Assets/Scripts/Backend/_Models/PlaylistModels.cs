using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Backend
{
    [Serializable]
    public class SelectedPlaylistId
    {
        public string Id { get; set; }
        public bool IsUserPlaylist { get; set; }
    }

    [Serializable]
    public class PlaylistSummaryResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string media_type { get; set; }
        public bool is_public { get; set; }
        public List<string> outcomes { get; set; }
        public List<int> age_groups { get; set; }
        public string cover_image_url { get; set; }
        public int owner { get; set; }
        public string owner_name { get; set; }
        public int total_media { get; set; }
        public int? total_duration { get; set; }
        public bool is_archived { get; set; }
        public DateTime? archived_at { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    [Serializable]
    public class PaginatedPlaylistResponse
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public List<PlaylistSummaryResponse> results { get; set; }
    }

    [Serializable]
    public class AssetUrls
    {
        public string original_url { get; set; }
        public string thumbnail_url { get; set; }
    }

    [Serializable]
    public class PlaylistDetailResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string media_type { get; set; }
        public bool is_public { get; set; }
        public List<string> outcomes { get; set; }
        public List<int> age_groups { get; set; }
        public string cover_image_url { get; set; }
        public string owner { get; set; }
        public string owner_name { get; set; }
        public int total_media { get; set; }
        public int? total_duration { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public List<PlaylistMediaItem> media_items { get; set; }
    }

    [Serializable]
    public class PlaylistMediaItem
    {
        public int order { get; set; }
        public DateTime added_at { get; set; }
        public string media_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int duration { get; set; }
        public AssetUrls asset_urls { get; set; }
    }

    [Serializable]
    public class PlaylistCreateRequest
    {
        public string name { get; set; }
        public string description { get; set; }
        public string media_type { get; set; }
        public bool is_public { get; set; }
        public string upload_session_id { get; set; }
        public string s3_key { get; set; }
    }

    [Serializable]
    public class PlaylistCreateResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string media_type { get; set; }
        public bool is_public { get; set; }
        public List<string> outcomes { get; set; }
        public List<string> age_groups { get; set; }
        public string cover_image_url { get; set; }
        public int owner { get; set; }
        public string owner_name { get; set; }
        public int total_media { get; set; }
        public int total_duration { get; set; }
        public bool is_archived { get; set; }
        public string archived_at { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    [Serializable]
    public class PlaylistUpdateRequest
    {
        public string name { get; set; }
        public string description { get; set; }
        public string media_type { get; set; }
        public bool is_public { get; set; }
        public List<string> outcomes { get; set; }
        public List<int> age_groups { get; set; }

        public string ToJson()
        {
            JObject jsonObject = new JObject();

            if (!string.IsNullOrEmpty(name))
                jsonObject["name"] = name;

            if (!string.IsNullOrEmpty(description))
                jsonObject["description"] = description;

            if (!string.IsNullOrEmpty(media_type))
                jsonObject["media_type"] = media_type;

            // Always include bool if it’s relevant (optional: you can add condition if needed)
            jsonObject["is_public"] = is_public;

            if (outcomes != null && outcomes.Count > 0)
                jsonObject["outcomes"] = JArray.FromObject(outcomes);

            if (age_groups != null && age_groups.Count > 0)
                jsonObject["age_groups"] = JArray.FromObject(age_groups);

            return jsonObject.ToString();
        }
    }

    [Serializable]
    public class PlaylistMediaModifyRequest
    {
        public List<string> media_ids { get; set; }
    }

    [Serializable]
    public class PlaylistMediaModifyResponse
    {
        public string id { get; set; }
        public string media { get; set; }
        public string media_name { get; set; }
        public int order { get; set; }
        public DateTime added_at { get; set; }
    }

    [Serializable]
    public class PlaylistReorderItem
    {
        public string media_id { get; set; }
        public int new_order { get; set; }
    }

    [Serializable]
    public class PlaylistReorderRequest
    {
        public List<PlaylistReorderItem> medias { get; set; }
    }

    [Serializable]
    public class PlaylistMediaReorderResponse
    {
        public List<PlaylistReorderItem> medias { get; set; }
    }

    [Serializable]
    public class PlaylistUploadSessionRequest
    {
        public string cover_image_name { get; set; }
    }

    [Serializable]
    public class PlaylistUploadConfirmRequest
    {
        public string s3_key { get; set; }
    }


    [Serializable]
    public class PlaylistUploadSessionResponse
    {
        public string upload_uuid { get; set; }
        public Media media { get; set; }
    }

    public class Media
    {
        public string cover_image_key { get; set; }
        public string content_type { get; set; }
        public string presigned_url { get; set; }
    }
}
