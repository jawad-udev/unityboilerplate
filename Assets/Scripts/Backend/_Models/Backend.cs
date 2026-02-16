// using System;
// using System.Collections.Generic;
// using Newtonsoft.Json;
// using UnityEngine;
// using UnityEngine.Analytics;
// using Backend;

// [System.Serializable]
// public class AudioClipData
// {
//     public string audioName;
//     public AudioClip audioClip;

//     // Returns display-friendly audio name
//     public string GetName()
//     {
//         if (string.IsNullOrEmpty(audioName) && GetAudioClip() != null)
//             return GetAudioClip().name;

//         return audioName;
//     }

//     // Returns display-friendly audio name
//     public AudioClip GetAudioClip()
//     {
//         return audioClip;
//     }

//     // Returns duration in 00:00 format
//     public string GetDuration()
//     {
//         if (GetAudioClip() == null)
//             return "00:00";

//         // Always ceil so even 0.1 sec shows as 1 second
//         int totalSeconds = Mathf.CeilToInt(GetAudioClip().length);

//         // Edge case: if clip has length but rounds to 0, force 1
//         if (totalSeconds == 0 && GetAudioClip().length > 0f)
//             totalSeconds = 1;

//         int minutes = totalSeconds / 60;
//         int seconds = totalSeconds % 60;

//         return $"{minutes:00}:{seconds:00}";
//     }
// }

// [System.Serializable]
// public class LoginResponse
// {
//     public string access_token;
//     public string refresh_token;
//     public string token_type;
//     public User user;
//     public Settings settings;
//     public Subscription subscription;
//     public bool is_active_subscription;
//     public bool force_update;
//     public bool is_approved;
//     public string rmt_status;
// }

// [System.Serializable]
// public class EmotionCounter
// {
//     public int Happy;
//     public int Sad;
//     public int Worried;
//     public int Angry;
//     public int Scared;

//     public EmotionCounter()
//     {
//         Happy = 0;
//         Sad = 0;
//         Worried = 0;
//         Angry = 0;
//         Scared = 0;
//     }
// }

// public class RefreshTokenData
// {
//     public string access_token;
//     public string refresh_token;
//     public string token_type;
// }

// [System.Serializable]
// public class AdditionalProfile
// {
//     public List<string> activities_like;
//     public List<string> diagnosis;

//     public AdditionalProfile()
//     {
//         activities_like = new List<string>();
//         diagnosis = new List<string>();
//     }
// }

// [System.Serializable]
// public class Settings
// {
// }

// [System.Serializable]
// public class Subscription
// {
//     public string name;
//     public string start_date;
//     public string end_date;
// }

// [System.Serializable]
// public class UpdateProfile
// {
//     public string email;
//     public string full_name;
//     public string gender;
//     public string date_of_birth;
// }

// [System.Serializable]
// public class UpdateProfileAdditional
// {
//     public List<string> activities_like;
//     public List<string> diagnosis;
// }

// public class ForgotPasswordResponse
// {
//     public int expiry_minutes;
// }

// public class EmotionStatData
// {
//     [JsonProperty("ANGRY", DefaultValueHandling = DefaultValueHandling.Populate)]
//     public int ANGRY = 0;

//     [JsonProperty("HAPPY", DefaultValueHandling = DefaultValueHandling.Populate)]
//     public int HAPPY = 0;

//     [JsonProperty("SAD", DefaultValueHandling = DefaultValueHandling.Populate)]
//     public int SAD = 0;

//     [JsonProperty("SCARED", DefaultValueHandling = DefaultValueHandling.Populate)]
//     public int SCARED = 0;

//     [JsonProperty("WORRIED", DefaultValueHandling = DefaultValueHandling.Populate)]
//     public int WORRIED = 0;

// }

// public class AddSubscriptionData
// {
//     public int expiry_minutes;
// }

// public class SubscriptionVerification
// {
//     public SubscriptionData subscription;
// }

// public class SubscriptionData
// {
//     public int user_id;
//     public int subscription_type_id;
//     public string start_date;
//     public string end_date;
// }

// public class UserSubscription
// {
//     public int user_id;
//     public int subscription_type_id;
//     public string start_date;
//     public string end_date;
// }

// public class Parent
// {
//     public int id;
//     public string first_name;
//     public string last_name;
//     public string email;
//     public string type;
//     public List<User> children;
//     public string refresh;
//     public string access;
//     public string subscription;
//     public string end_date;
// }

// public class RedeemResponse
// {
//     public int user_id;
//     public string subscription_type;
//     public string start_date;
//     public string end_date;
// }

// public class VoiceData
// {
//     public string encoded_data;

//     public VoiceData(string encoded_data)
//     {
//         this.encoded_data = encoded_data;
//     }
// }

// public class AudioFile
// {
//     public string path;
//     public int status_code;
//     public string filename;
//     public bool send_header_only;
//     public string media_type;
//     public object background;
//     public List<List<string>> raw_headers;
//     public Headers _headers;
//     public object stat_result;
// }

// public class ProcessedAudio
// {
//     public List<PhonesDatum> phones_data;
//     public string emotion;
//     public AudioFile audio_file;
// }

// public class Headers
// {
//     [JsonProperty("content-type")]
//     public string contenttype;

//     [JsonProperty("content-disposition")]
//     public string contentdisposition;
// }

// public class PhonesDatum
// {
//     public string images_path;
//     public float duration;
//     public int frame_count;
//     public float percentage;
// }

// public class ProcessedAudioByte
// {
//     public string audio_type;
//     public string audio_encoded_data;
//     public string bezzie_emotion;
//     public List<PhonesDatum> phones_data;
//     public string child_emotion;
//     public bool next_step;
// }

// public class Homework
// {
//     public string command;
//     public int days;
//     public string created_at;
// }