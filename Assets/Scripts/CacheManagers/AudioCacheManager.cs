using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Audio cache manager:
/// - downloads audio and caches original bytes (with correct extension),
/// - tries to load cached WAV files with an internal WAV parser (no external WavUtility required),
/// - falls back to UnityWebRequest for other formats,
/// - writes files atomically to avoid partial file reads.
/// </summary>
public static class AudioCacheManager
{
    private static readonly string cacheFolder = Path.Combine(Application.persistentDataPath, "AudioCache");

    /// <summary>
    /// Main entry. Returns AudioClip from cache or network.
    /// </summary>
    public static async Task<AudioClip> GetAudioClip(string url, Action<float> progress = null)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("[AudioCache] URL is null/empty.");
            return null;
        }

        EnsureCacheFolder();

        string ext = GetExtensionFromUrl(url);
        AudioType type = GetAudioTypeFromExtension(ext);
        string fileName = $"{url.GetHashCode()}{ext}";
        string filePath = Path.Combine(cacheFolder, fileName);

        if (File.Exists(filePath))
        {
            Debug.Log($"[AudioCache] Found cache: {filePath} (size {new FileInfo(filePath).Length} bytes)");
            AudioClip fromCache = await LoadAudioClipFromFile(filePath, type, url);
            if (fromCache != null)
                return fromCache;

            Debug.LogWarning("[AudioCache] Cache load failed - will re-download.");
        }

        return await DownloadAndCacheUsingFileDownloader(url, filePath, type, progress);
    }

    /// <summary>
    /// Save an in-memory clip as WAV for the given URL (atomic).
    /// </summary>
    public static bool SaveClipForUrl(string url, AudioClip clip)
    {
        if (string.IsNullOrEmpty(url) || clip == null)
        {
            Debug.LogError("[AudioCache] SaveClipForUrl: invalid args.");
            return false;
        }

        EnsureCacheFolder();
        string ext = GetExtensionFromUrl(url);
        string filePath = Path.Combine(cacheFolder, $"{url.GetHashCode()}{ext}");
        string tmp = filePath + ".tmp";

        try
        {
            byte[] wavBytes;

            if (ext.Equals(".ogg"))
                wavBytes = OggVorbis.VorbisPlugin.GetOggVorbis(clip);
            else
                wavBytes = ConvertAudioClipToWav(clip);

            File.WriteAllBytes(tmp, wavBytes);
            if (File.Exists(filePath)) File.Delete(filePath);
            File.Move(tmp, filePath);
            Debug.Log($"[AudioCache] Saved WAV clip: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AudioCache] SaveClipForUrl failed: {ex}");
            if (File.Exists(tmp)) File.Delete(tmp);
            return false;
        }
    }

    // Add this method inside your AudioCacheManager class
    private static async Task<AudioClip> DownloadAndCacheUsingFileDownloader(
        string url,
        string filePath,
        AudioType type,
        Action<float> progress = null)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("[AudioCache] DownloadAndCacheUsingFileDownloader: URL is null/empty.");
            progress?.Invoke(0f);
            return null;
        }

        string tmp = filePath + ".tmp";

        try
        {
            // Ensure tmp doesn't exist from previous failed runs
            if (File.Exists(tmp)) File.Delete(tmp);

            using (UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
            {
                // DownloadHandlerFile writes directly to disk (no big memory)
                www.downloadHandler = new DownloadHandlerFile(tmp);

                var op = www.SendWebRequest();

                // Poll progress while request is in flight
                while (!op.isDone)
                {
                    // Unity's downloadProgress is a float [0..1]
                    float prog = www.downloadProgress; // may stay 0 if server doesn't provide content-length
                    Debug.Log($"[AudioCache] Download progress: {prog:P1} for {url}");
                    try { progress?.Invoke(prog); } catch { /* swallow callback exceptions */ }

                    await Task.Yield();
                }

#if UNITY_2020_1_OR_NEWER
                if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
                {
                    Debug.LogError($"[AudioCache] Download failed: {www.error}");
                    // cleanup tmp file
                    if (File.Exists(tmp)) File.Delete(tmp);
                    progress?.Invoke(0f);
                    return null;
                }

                // Final progress update
                progress?.Invoke(1f);
                Debug.Log("[AudioCache] Download complete, moving temp file to final path.");

                // Atomically move tmp -> final
                if (File.Exists(filePath)) File.Delete(filePath);
                File.Move(tmp, filePath);

                Debug.Log($"[AudioCache] Saved to {filePath} ({new FileInfo(filePath).Length} bytes)");
            }

            // Now create AudioClip depending on extension
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            if (ext == ".wav")
            {
                try
                {
                    // Read bytes and parse via your WAV parser (must exist in the class)
                    byte[] b = File.ReadAllBytes(filePath);
                    AudioClip clip = ParseWavFileToAudioClip(b, Path.GetFileNameWithoutExtension(filePath));
                    if (clip != null)
                    {
                        Debug.Log("[AudioCache] Created AudioClip from WAV bytes.");
                        return clip;
                    }
                    else
                    {
                        Debug.LogWarning("[AudioCache] WAV parser returned null, falling back to UnityWebRequest loader.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[AudioCache] Exception parsing WAV bytes: {ex}");
                }
            }

            // Fallback: let Unity decode the local file (good for MP3/OGG/etc)
            try
            {
                using (UnityWebRequest r = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, type))
                {
                    var op2 = r.SendWebRequest();
                    while (!op2.isDone)
                    {
                        // You can also forward this progress if you want:
                        float p = r.downloadProgress;
                        Debug.Log($"[AudioCache] Local decode progress: {p:P1}");
                        try { progress?.Invoke(p); } catch { }
                        await Task.Yield();
                    }

#if UNITY_2020_1_OR_NEWER
                    if (r.result != UnityWebRequest.Result.Success)
#else
                if (r.isNetworkError || r.isHttpError)
#endif
                    {
                        Debug.LogError($"[AudioCache] Local GetAudioClip failed: {r.error}");
                        progress?.Invoke(0f);
                        return null;
                    }

                    AudioClip clip = DownloadHandlerAudioClip.GetContent(r);
                    Debug.Log("[AudioCache] Loaded AudioClip via UnityWebRequest from file://");
                    progress?.Invoke(1f);
                    return clip;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AudioCache] Exception during local decode: {ex}");
                progress?.Invoke(0f);
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AudioCache] Unexpected exception in DownloadAndCacheUsingFileDownloader: {ex}");
            if (File.Exists(tmp)) File.Delete(tmp);
            progress?.Invoke(0f);
            return null;
        }
    }

    /// <summary>
    /// Attempts several robust methods to load a local file:
    /// 1) If WAV, parse bytes with internal WAV parser -> AudioClip
    /// 2) Try UnityWebRequest with file://
    /// 3) If both fail, optionally re-download is attempted by the caller (we return null here)
    /// </summary>
    private static async Task<AudioClip> LoadAudioClipFromFile(string filePath, AudioType type, string originalUrl)
    {
        if (!File.Exists(filePath))
            return null;

        string ext = Path.GetExtension(filePath).ToLowerInvariant();

        // 1) If WAV, try internal parser
        if (ext == ".wav")
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                Debug.Log($"[AudioCache] Attempting internal WAV parse ({fileBytes.Length} bytes)");
                AudioClip clip = ParseWavFileToAudioClip(fileBytes, Path.GetFileNameWithoutExtension(filePath));
                if (clip != null)
                {
                    Debug.Log("[AudioCache] Parsed WAV successfully via internal parser.");
                    return clip;
                }
                else
                {
                    Debug.LogWarning("[AudioCache] Internal WAV parser returned null.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AudioCache] Exception parsing WAV: {ex}");
            }
        }

        // 2) Fallback: UnityWebRequest file://
        try
        {
            Debug.Log($"[AudioCache] Trying UnityWebRequest for file://{filePath} (type: {type})");
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, type))
            {
                var op = www.SendWebRequest();
                while (!op.isDone)
                    await Task.Yield();

#if UNITY_2020_1_OR_NEWER
                if (www.result == UnityWebRequest.Result.Success)
#else
                if (!www.isNetworkError && !www.isHttpError)
#endif
                {
                    try
                    {
                        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                        if (clip != null)
                        {
                            Debug.Log("[AudioCache] Loaded cached clip via UnityWebRequest.");
                            return clip;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[AudioCache] GetContent from file request threw: {ex}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[AudioCache] UnityWebRequest load local failed: {www.error}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[AudioCache] Exception while UnityWebRequest loading local file: {ex}");
        }

        // 3) Fail: return null so caller can decide to re-download
        return null;
    }

    #region Utilities: WAV parsing + writing (self-contained)

    // Parse WAV bytes and return an AudioClip (supports PCM16, PCM8, IEEE float32)
    private static AudioClip ParseWavFileToAudioClip(byte[] fileBytes, string name)
    {
        if (fileBytes == null || fileBytes.Length < 44) return null;

        try
        {
            int offset = 0;
            // RIFF header
            string riff = GetString(fileBytes, ref offset, 4);
            if (riff != "RIFF") return null;
            offset += 4; // chunk size (skip)
            string wave = GetString(fileBytes, ref offset, 4);
            if (wave != "WAVE") return null;

            // Read chunks until 'fmt ' and 'data' are found
            int channels = 0;
            int sampleRate = 0;
            int bitsPerSample = 0;
            ushort audioFormat = 0;
            int dataChunkPos = -1;
            int dataChunkSize = 0;

            while (offset + 8 <= fileBytes.Length)
            {
                string chunkId = GetString(fileBytes, ref offset, 4);
                int chunkSize = BitConverter.ToInt32(fileBytes, offset);
                offset += 4;

                if (chunkId == "fmt ")
                {
                    audioFormat = BitConverter.ToUInt16(fileBytes, offset);
                    channels = BitConverter.ToUInt16(fileBytes, offset + 2);
                    sampleRate = BitConverter.ToInt32(fileBytes, offset + 4);
                    // int byteRate = BitConverter.ToInt32(fileBytes, offset + 8);
                    // ushort blockAlign = BitConverter.ToUInt16(fileBytes, offset + 12);
                    bitsPerSample = BitConverter.ToUInt16(fileBytes, offset + 14);
                    // move offset to next chunk
                    offset += chunkSize;
                }
                else if (chunkId == "data")
                {
                    dataChunkPos = offset;
                    dataChunkSize = chunkSize;
                    break; // we can stop here
                }
                else
                {
                    // skip other chunk
                    offset += chunkSize;
                }
            }

            if (dataChunkPos < 0 || dataChunkSize <= 0 || channels <= 0 || sampleRate <= 0) return null;

            int bytesPerSample = bitsPerSample / 8;
            int totalSamples = dataChunkSize / (bytesPerSample * channels);

            float[] floatData = new float[totalSamples * channels];

            int srcPos = dataChunkPos;
            for (int i = 0; i < totalSamples; i++)
            {
                for (int ch = 0; ch < channels; ch++)
                {
                    int sampleIndex = (i * channels) + ch;
                    float sample = 0f;

                    if (audioFormat == 1) // PCM
                    {
                        if (bitsPerSample == 16)
                        {
                            short s = BitConverter.ToInt16(fileBytes, srcPos);
                            sample = s / 32768f;
                            srcPos += 2;
                        }
                        else if (bitsPerSample == 8)
                        {
                            byte b = fileBytes[srcPos];
                            sample = (b - 128) / 128f;
                            srcPos += 1;
                        }
                        else
                        {
                            // unsupported PCM bits
                            return null;
                        }
                    }
                    else if (audioFormat == 3) // IEEE float
                    {
                        if (bitsPerSample == 32)
                        {
                            sample = BitConverter.ToSingle(fileBytes, srcPos);
                            srcPos += 4;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        // unsupported audio format
                        return null;
                    }

                    floatData[sampleIndex] = sample;
                }
            }

            // Create AudioClip and set data
            AudioClip audioClip = AudioClip.Create(name, totalSamples, channels, sampleRate, false);
            audioClip.SetData(floatData, 0);
            return audioClip;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[AudioCache] WAV parse exception: {ex}");
            return null;
        }
    }

    // Convert AudioClip -> WAV bytes (PCM16)
    private static byte[] ConvertAudioClipToWav(AudioClip clip)
    {
        if (clip == null) throw new ArgumentNullException(nameof(clip));

        int channels = clip.channels;
        int sampleRate = clip.frequency;
        int samples = clip.samples; // samples per channel
        float[] data = new float[samples * channels];
        clip.GetData(data, 0);

        short[] intData = new short[data.Length];
        byte[] bytesData = new byte[data.Length * 2];

        const float rescaleFactor = 32767f;

        for (int i = 0; i < data.Length; i++)
        {
            float v = Mathf.Clamp(data[i], -1f, 1f);
            intData[i] = (short)(v * rescaleFactor);
            byte[] byteArr = BitConverter.GetBytes(intData[i]);
            bytesData[i * 2] = byteArr[0];
            bytesData[i * 2 + 1] = byteArr[1];
        }

        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            // RIFF header
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + bytesData.Length); // file size - 8
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

            // fmt chunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16); // fmt chunk length
            writer.Write((short)1); // audio format PCM
            writer.Write((short)channels);
            writer.Write(sampleRate);
            int byteRate = sampleRate * channels * 2;
            writer.Write(byteRate);
            short blockAlign = (short)(channels * 2);
            writer.Write(blockAlign);
            writer.Write((short)16); // bits per sample

            // data chunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(bytesData.Length);
            writer.Write(bytesData);

            writer.Flush();
            return stream.ToArray();
        }
    }

    private static string GetString(byte[] bytes, ref int offset, int count)
    {
        string s = System.Text.Encoding.ASCII.GetString(bytes, offset, count);
        offset += count;
        return s;
    }

    #endregion

    #region Helpers

    private static string GetExtensionFromUrl(string url)
    {
        try
        {
            string ext = Path.GetExtension(url);
            if (string.IsNullOrEmpty(ext)) return ".wav";
            return ext.ToLowerInvariant();
        }
        catch { return ".wav"; }
    }

    private static AudioType GetAudioTypeFromExtension(string ext)
    {
        switch (ext)
        {
            case ".mp3": return AudioType.MPEG;
            case ".ogg": return AudioType.OGGVORBIS;
            case ".wav": return AudioType.WAV;
            case ".aiff":
            case ".aif": return AudioType.AIFF;
            default: return AudioType.UNKNOWN;
        }
    }

    private static void EnsureCacheFolder()
    {
        if (!Directory.Exists(cacheFolder))
            Directory.CreateDirectory(cacheFolder);
    }

    #endregion


    /// <summary>
    /// Deletes all files in the cache directory (non-blocking wrapper).
    /// </summary>
    public static async Task<bool> ClearAllCacheAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                if (!Directory.Exists(cacheFolder)) return true;
                var files = Directory.GetFiles(cacheFolder, "*", SearchOption.TopDirectoryOnly);
                int deleted = 0;
                foreach (var f in files)
                {
                    try
                    {
                        File.SetAttributes(f, FileAttributes.Normal);
                        File.Delete(f);
                        deleted++;
                    }
                    catch (Exception exFile)
                    {
                        Debug.LogWarning($"[AudioCache] Could not delete {f}: {exFile}");
                    }
                }
                Debug.Log($"[AudioCache] ClearAllCacheAsync deleted {deleted} files.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AudioCache] ClearAllCacheAsync failed: {ex}");
                return false;
            }
        });
    }

    public static bool DeleteCacheForUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return false;

        try
        {
            EnsureCacheFolder();
            string hash = url.GetHashCode().ToString();
            string[] matches = Directory.GetFiles(cacheFolder, hash + "*", SearchOption.TopDirectoryOnly);
            bool any = false;
            foreach (var f in matches)
            {
                try
                {
                    File.SetAttributes(f, FileAttributes.Normal);
                    File.Delete(f);
                    Debug.Log($"[AudioCache] Deleted cache file: {f}");
                    any = true;
                }
                catch (Exception exFile)
                {
                    Debug.LogWarning($"[AudioCache] Failed to delete file '{f}': {exFile}");
                }
            }
            return any;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AudioCache] DeleteCacheForUrl failed: {ex}");
            return false;
        }
    }
}
