using Newtonsoft.Json.Linq;

public static class BtJsonUtils
{
    public static JObject GetConfig(JObject json)
    {
        return json["config"] as JObject ?? json;
    }
}