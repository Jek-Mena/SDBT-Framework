// BtJsonValidator.cs
// Scope: Static, offline validation of Behavior Tree JSON files
// Does not depend on runtime (e.g., Blackboard, plugins, MonoBehaviours)
// Validates structure, config schema, known types, $ref resolution (static)

using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public static class BtJsonValidator
{
    private const string Hash = "#";
    
    public class ValidationResult
    {
        public List<string> Errors = new();
        public List<string> Warnings = new();
        public bool IsValid => Errors.Count == 0;
    }

    public static ValidationResult ValidateFromFile(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var root = JObject.Parse(json);
        return ValidateFromJObject(root);
    }

    public static ValidationResult ValidateFromJObject(JObject root)
    {
        var result = new ValidationResult();
        var knownRefs = new HashSet<string>();

        // Capture top-lefel $refs
        foreach (var prop in root.Properties())
        {
            if(prop.Name != CoreKeys.Root)
                knownRefs.Add(Hash + prop.Name);
        }

        if(!root.TryGetValue(CoreKeys.Root, out var rootNode))
        {
            result.Errors.Add($"Missing {CoreKeys.Root} node in JSON.");
            return result;
        }

        ValidateNode(rootNode, CoreKeys.Root, knownRefs, root, result);
        return result;
    }

    private static void ValidateNode(JToken node, string path, HashSet<string> knownRefs, JObject root, ValidationResult result)
    {
        if (node is not JObject obj)
        {
            result.Errors.Add($"[{path}] Node is not an object.");
            return;
        }

        // Handle $ref
        if (obj.TryGetValue(CoreKeys.Ref, out var refToken))
        {
            var refPath = refToken.ToString();
            if (!refPath.StartsWith(Hash))
                result.Errors.Add($"[{path}] Invalid $ref format: {refPath}");
            else if (!knownRefs.Contains(refPath))
                result.Errors.Add($"[{path}] $ref '{refPath}' not found in document.");
            return;
        }

        // Validate node type
        if (!obj.TryGetValue(CoreKeys.Type, out var typeToken))
        {
            result.Errors.Add($"[{path}] Missing required field: either 'type' or 'btKey' must be present.");
            return;
        }
        
        var typeKey = typeToken.ToString();
        if (!BtNodeRegistry.IsKnown(typeKey))
        {
            result.Warnings.Add($"[{path}] Unknown node type '{typeKey}'.");
        }

        var config = obj[CoreKeys.Config] as JObject;
        if (config is JObject cfg && cfg.TryGetValue(CoreKeys.Ref, out var cfgRef))
        {
            var cfgRefPath = cfgRef.ToString();
            if (!cfgRefPath.StartsWith(Hash))
                result.Errors.Add($"[{path}].config] Invalid $ref format: {cfgRefPath}");
            else if (!knownRefs.Contains(cfgRefPath))
                result.Errors.Add($"[{path}].config] $ref '{cfgRefPath}' not found in document.");
        }
        
        // Basic shape rules (optional)
        if (obj.TryGetValue(CoreKeys.Children, out var childrenToken) && childrenToken is JArray childrenArray)
        {
            for (int i = 0; i < childrenArray.Count; i++)
            {
                ValidateNode(childrenArray[i], $"{path}.{CoreKeys.Children}[{i}]", knownRefs, root, result);
            }
        }

        if (obj.TryGetValue(CoreKeys.Child, out var childToken))
        {
            ValidateNode(childToken, $"{path}.{CoreKeys.Child}", knownRefs, root, result);
        }

        if (BtNodeSchemaRegistry.TryGet(typeKey, out var schema))
        {
            schema.Validate(config, path, result);
        }
    }
}