using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class BtTreeBuilder
{
    public static IBehaviorNode Build(JToken json, Blackboard blackboard)
    {
        // Recursively build with the correct delegate signature
        return Build(new TreeNodeData((JObject)json), blackboard, nodeData => Build(nodeData, blackboard));
    }

    public static IBehaviorNode Build(TreeNodeData nodeData, Blackboard blackboard, Func<TreeNodeData, IBehaviorNode> recurse)
    {
        var alias = nodeData.BtKey;
        if (string.IsNullOrEmpty(alias))
            throw new Exception($"[BtTreeBuilder] Missing or empty 'type'/'btKey' in node: {nodeData}");

        var factory = BtNodeRegistry.GetFactoryByAlias(alias);
        var node = factory.CreateNode(nodeData, blackboard, recurse);

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
    public static IBehaviorNode Load(string treeId, Blackboard blackboard)
    {
        var context = nameof(BtTreeBuilder);
        var path = $"Data/BTs/{treeId}";
        var textAsset = Resources.Load<TextAsset>(path);

        if (textAsset == null)
            throw new Exception($"[{context}] Could not load BT file at Resources/{path}.json");

        try
        {
            var fileJson = JObject.Parse(textAsset.text);
            var rootToken = fileJson[CoreKeys.Root] ?? fileJson;

            JsonUtils.ResolveRefs(rootToken, blackboard);
            
            if (rootToken is not JObject rootNode)
                throw new Exception($"[{context}] Invalid tree structure: '{CoreKeys.Root}' must be an object with a '{CoreKeys.Type}' field.");

            if (string.IsNullOrEmpty(rootNode[CoreKeys.Type]?.ToString()))
                throw new Exception($"[{context}] Missing or empty '{CoreKeys.Type}' in root node.");

            return BtTreeBuilder.Build(rootNode, blackboard);
        }
        catch (Exception ex)
        {
            throw new Exception($"[{context}] Failed to parse behavior tree '{treeId}': {ex.Message}", ex);
        }
    }

    // Overload for recursive calls with TreeNodeData
    private static IBehaviorNode Build(TreeNodeData nodeData, Blackboard blackboard)
    {
        // Recursion with correct delegate signature
        return Build(nodeData, blackboard, childNodeData => Build(childNodeData, blackboard));
    }
}