using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AI.BehaviorTree.Registry.List
{
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

            var registeredBTs = new List<string>();
            
            foreach (var asset in textAssets)
            {
                var btJson = JObject.Parse(asset.text);
                var key = Path.GetFileNameWithoutExtension(asset.name);
                BtConfigRegistry.RegisterTemplate(key, btJson);
                registeredBTs.Add($"'{key}' (from '{asset.name}')");
            }
            
            var summary = string.Join(",\n  ", registeredBTs);
            Debug.Log($"[{ScriptName}] Bootstrap complete. \nRegistered BTs:\n  {summary}\nTotal: {registeredBTs.Count}");
        }
    }
}