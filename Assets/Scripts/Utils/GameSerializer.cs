using Newtonsoft.Json;

public class GameSerializer
{
    public static T Deserialize<T>(string objectStr)
    {
        return JsonConvert.DeserializeObject<T>(objectStr);
    }


    public static string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }


    public static string Serialize(object obj, bool ignoreNull)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include;

        return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
    }
}

