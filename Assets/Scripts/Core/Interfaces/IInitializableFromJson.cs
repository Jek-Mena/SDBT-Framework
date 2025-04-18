using Newtonsoft.Json.Linq;

public interface IInitializableFromJson
{
    void Initialize(JObject config);
}