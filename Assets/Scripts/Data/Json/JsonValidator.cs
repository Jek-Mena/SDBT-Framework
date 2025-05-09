using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class JsonValidator
{
    public static void ValidateJsonFiles(params string[] paths)
    {
        foreach (var path in paths)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Debug.LogWarning($"[JsonValidator] File does not exist: {path}");
                    continue;
                }

                var text = File.ReadAllText(path);
                JToken.Parse(text);
                Debug.Log($"[JsonValidator] ✅ Valid: {path}");
            }
            catch (JsonReaderException ex)
            {
                Debug.LogError($"[JsonValidator] ❌ Invalid JSON in {path}: {ex.Message}");
            }
        }
    }
}