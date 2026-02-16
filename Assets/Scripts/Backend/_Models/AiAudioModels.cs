using System;
using System.Collections.Generic;

namespace Backend
{
    [Serializable]
    public class UploadUrlRequest
    {
        public string file_name;
        public string content_type;
    }

    [Serializable]
    public class UploadUrlData
    {
        public string job_id;
        public string upload_url;
        public string s3_key;
        public string content_type;
        public string status;
    }

    [Serializable]
    public class JobStatusData
    {
        public string id { get; set; }
        public string status { get; set; }
        public FilesInfo files_info { get; set; }
    }

    [Serializable]
    public class FilesInfo
    {
        public Music music { get; set; }
        public Vocals vocals { get; set; }
        public Json json { get; set; }
    }

    [Serializable]
    public class Json
    {
        public string url { get; set; }
        public string key { get; set; }
        public string note { get; set; }
    }

    [Serializable]
    public class Music
    {
        public string url { get; set; }
        public string key { get; set; }
    }

    [Serializable]
    public class Vocals
    {
        public string url { get; set; }
        public string key { get; set; }
    }
}