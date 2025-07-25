using System.IO;
using AI.BehaviorTree.Registry.List;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public static class BtJsonBulkValidator
{
    private const string TargetFolder = "Assets/Resources/BT"; // Customize this as needed
    
    [MenuItem("Tools/Behavior Tree/Validate All JSONs")]
    public static void ValidateAll()
    {
        // Ensure schema is available
        BtNodeSchemaRegistrationList.InitializeDefaults();

        var files = Directory.GetFiles(TargetFolder, "*.json", SearchOption.AllDirectories);

        var validCount = 0;
        var invalidCount = 0;

        foreach (var filePath in files)
        {
            try
            {
                var text = File.ReadAllText(filePath);
                var json = JObject.Parse(text);
                var result = BtJsonValidator.ValidateFromJObject(json);

                if (!result.IsValid)
                {
                    Debug.LogError($"❌ INVALID: {filePath}");
                    foreach (var error in result.Errors)
                        Debug.LogError("   - " + error);

                    invalidCount++;
                }
                else
                {
                    Debug.Log($"✔ VALID: {filePath}");
                    validCount++;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"EXCEPTION in {filePath}:\n{ex.Message}");
                invalidCount++;
                
            }
        }

        Debug.Log($"[BT Validator] Finished: {validCount} valid / {invalidCount} invalid / {files.Length} total");
    }
}