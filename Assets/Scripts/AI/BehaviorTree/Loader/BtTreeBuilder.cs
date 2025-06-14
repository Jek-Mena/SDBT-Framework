using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class BtTreeBuilder
{
    // Use this to handle string or inline object format from any plugin or context
    public static IBehaviorNode LoadFromToken(JToken treeToken, BtContext context)
    {
        var scriptName = nameof(BtTreeBuilder);

        if (treeToken.Type == JTokenType.String)
        {
            var treeId = treeToken.ToString();
            return Load(treeId, context);
        }
        else if (treeToken.Type == JTokenType.Object)
        {
            var obj = (JObject)treeToken;
            var rootToken = obj[CoreKeys.Root] ?? obj;
            
            JsonUtils.ResolveRefs(rootToken, context);
            Debug.Assert(!JsonUtils.HasUnresolvedRefs(rootToken), $"[{scriptName}] Unresolved {CoreKeys.Ref} found post-resolution.");
            
            BtProfileResolver.ResolveAllProfiles(rootToken as JObject, context);
            
            return Build(rootToken, context);
        }
        else
        {
            throw new Exception($"[{scriptName}] Invalid tree structure:  must be string (tree ID) or object (inline BT).");
        }
    }
    
    private static IBehaviorNode Build(JToken json, BtContext context)
    {
        // Recursively build with the correct delegate signature
        return Build(new TreeNodeData((JObject)json), context, nodeData => Build(nodeData, context));
    }
    
    // Overload for recursive calls with TreeNodeData
    private static IBehaviorNode Build(TreeNodeData nodeData, BtContext context)
    {
        // Recursion with the correct delegate signature
        return Build(nodeData, context, childNodeData => Build(childNodeData, context));
    }
    
    private static IBehaviorNode Build(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> recurse)
    {
        var alias = nodeData.BtType;
        if (string.IsNullOrEmpty(alias))
            throw new Exception($"[BtTreeBuilder] Missing or empty 'type'/'btKey' in node: {nodeData}");

        var factory = BtNodeRegistry.GetFactoryByAlias(alias);
        var node = factory.CreateNode(nodeData, context, recurse);

        // Wrap with lifecycle if needed
        if (node is IExitableBehavior)
            node = new BtLifecycleNode(node);

        return node;
    }
    
    /// <summary>
    /// Loads a behavior tree from a JSON text asset under Resources/BTrees/{treeId}.json.
    /// Supports both:
    /// - Wrapped format: { "name": "treeName", "root": { ... } }
    /// - Raw node format: { "type": "Bt/Sequence", ... }
    /// </summary>
    private static IBehaviorNode Load(string treeId, BtContext context)
    {
        var scriptName = nameof(BtTreeBuilder);
        var path = $"Data/BTs/{treeId}";
        var textAsset = Resources.Load<TextAsset>(path);

        if (textAsset == null)
            throw new Exception($"[{scriptName}] Could not load BT file at Resources/{path}.json");

        try
        {
            var fileJson = JObject.Parse(textAsset.text);
            var rootToken = fileJson[CoreKeys.Root] ?? fileJson;

            JsonUtils.ResolveRefs(rootToken, context);
            Debug.Assert(!JsonUtils.HasUnresolvedRefs(rootToken), $"[{scriptName}] Unresolved {CoreKeys.Ref} found post-resolution.");
            
            if (rootToken is not JObject rootNode)
                throw new Exception($"[{scriptName}] Invalid tree structure: '{CoreKeys.Root}' must be an object with a '{CoreKeys.Type}' field.");

            if (string.IsNullOrEmpty(rootNode[CoreKeys.Type]?.ToString()))
                throw new Exception($"[{scriptName}] Missing or empty '{CoreKeys.Type}' in root node.");
            
            return Build(rootNode, context);
        }
        catch (Exception ex)
        {
            throw new Exception($"[{scriptName}] Failed to parse behavior tree '{treeId}': {ex.Message}", ex);
        }
    }
}