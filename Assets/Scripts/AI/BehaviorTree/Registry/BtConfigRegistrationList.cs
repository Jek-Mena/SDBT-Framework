using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class BtConfigRegistrationList
{
    private const string ScriptName = nameof(BtConfigRegistrationList);
    private static bool _hasInitialized;
    
    public static void Initialize()
    {
        if (_hasInitialized) return;
        _hasInitialized = true;

        var configFolder = "Data/BTs"; // Resources path
        var textAssets = Resources.LoadAll<TextAsset>(configFolder);

        foreach (var asset in textAssets)
        {
            var btJson = JObject.Parse(asset.text);
            var key = Path.GetFileNameWithoutExtension(asset.name);
            BtConfigRegistry.RegisterTemplate(key, btJson);
            Debug.Log($"[{ScriptName}] Registered BT '{key}' from '{asset.name}'");
        }
        
        Debug.Log($"[{ScriptName}] Bootstrap complete. Total of {textAssets.Length} BTs registered.");
    }
}