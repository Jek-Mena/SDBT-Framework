using System;
using Newtonsoft.Json.Linq;

public static class BtConfigResolver
{
    public static JObject Resolve(JObject jObject, Blackboard blackboard, string context)
    {
        var refPath = jObject[CoreKeys.Config]?[CoreKeys.Ref]?.ToString();
        if (string.IsNullOrEmpty(refPath))
            throw new Exception($"[{context}] Missing or invalid [{CoreKeys.Ref}]: {refPath} in node config.");

        var configData = blackboard.Get<ConfigData>(PluginMetaKeys.Core.BtConfig.Plugin);
        if (configData?.RawJson == null)
            throw new Exception($"[{context}] BtConfig not found in blackboard.");

        var token = JsonUtils.ResolveDotPath(configData.RawJson, refPath, context);

        if (token is not JObject block)
            throw new Exception($"[{context}] Resolved config at '{refPath}', but it was not a valid object.");

        return block;
    }
    private static JToken ResolveDotPath(JObject root, string path)
    {
        var parts = path.Split('.');
        JToken current = root;

        foreach (var part in parts)
        {
            if (current is JObject obj && obj.TryGetValue(part, out var next))
                current = next;
            else
                throw new Exception($"[BtConfigResolver] Cannot resolve path '{path}' � failed at '{part}'");
        }

        return current;
    }
}