using System;
using System.Linq;
using AI.BehaviorTree.Core;
using AI.BehaviorTree.Runtime.Context;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class BtTreeBuilderExtension
{
    private const string ScriptName = nameof(BtTreeBuilderExtension);
    
    public static void ResolveRefs(JToken node, BtContext context)
    {
        // Get config root ONCE for performance; throw if missing.
        var configData = context.Definition.Config;
        if (configData == null)
            throw new Exception($"[{ScriptName}] Missing Entity/Agent config in context!");

        Debug.Log($"[{ScriptName}] Loading BT tree. Token: {node}, Type: {node.Type}");
        ResolveRefsRecursive(node, configData);
    }
    
    // Internal: recursive helper. DO NOT call this directly outside this file.
    private static void ResolveRefsRecursive(JToken node, JObject configRoot)
    {
        switch (node.Type)
        {
            case JTokenType.Object:
                foreach (var prop in ((JObject)node).Properties().ToList())
                {
                    if (prop.Value is JObject child && child.TryGetValue(BtJsonFields.Ref, out var refToken))
                        ((JObject)node)[prop.Name] = ResolveDotPath(configRoot, refToken.Value<string>(), "ResolveRefs");
                    else
                        ResolveRefsRecursive(prop.Value, configRoot);
                }
                break;
            case JTokenType.Array:
                foreach (var item in node.Children())
                {
                    // Only recurse into objects or arrays—not primitives!
                    if (item.Type is JTokenType.Object or JTokenType.Array)
                        ResolveRefsRecursive(item, configRoot);
                }
                break;
            default:
                // Primitive value (string, int, float, bool, etc.) — nothing to do here.
                // This is a leaf value in a config, so we do not recurse.
                break;
        }
    }
    
    private static JToken ResolveDotPath(JObject root, string path, string context)
    {
        var parts = path.Split('.');
        JToken current = root;

        foreach (var part in parts)
        {
            if (current is JObject obj && obj.TryGetValue(part, out var next))
                current = next;
            else
                throw new Exception($"[{context}] Cannot resolve path '{path}' — failed at '{part}'");
        }

        return current;
    }
    
    /// <summary>
    /// Determines whether a JToken contains any unresolved references.
    /// Considers a reference unresolved if it contains a key matching CoreKeys.Ref
    /// or has nested tokens that do.
    /// </summary>
    public static bool HasUnresolvedRefs(JToken token)
    {
        return token.Type switch
        {
            JTokenType.Object => ((JObject)token).ContainsKey(BtJsonFields.Ref) ||
                                 ((JObject)token).Properties().Any(p => HasUnresolvedRefs(p.Value)),
            JTokenType.Array => token.Children().Any(HasUnresolvedRefs),
            _ => false
        };
    }
}