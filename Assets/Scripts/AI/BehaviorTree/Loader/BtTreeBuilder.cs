using System;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.TemporalControl.Component;
using AI.BehaviorTree.Registry;
using AI.BehaviorTree.Runtime.Context;
using Keys;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AI.BehaviorTree.Loader
{
    public static class BtTreeBuilder
    {
        private const string ScriptName = nameof(BtTreeBuilder);

        public static IBehaviorNode LoadTreeFromToken(object treeToken, BtContext context)
        {

            // If it's already a JToken, pass through
            if (treeToken is JToken jt)
                return LoadTreeFromToken(jt, context);

            // Only accept string, fail on anything else (fail-fast)
            if (treeToken is string str)
                return LoadTreeFromToken(new JValue(str), context);

            throw new ArgumentException(
                $"[{ScriptName}] Invalid treeToken type: {treeToken.GetType().Name}. " +
                "Expected string (tree ID) or JToken (inline BT).", nameof(treeToken));
        }
        
        /// <summary>
        /// Loads a behavior tree from a JToken.
        /// - If the token is a string, treats it as a resource tree ID and loads from Unity Resources.
        /// - If the token is an object, treats it as an inline tree definition and builds it directly.
        /// Resolves all references and profiles before building the tree.
        /// Throws if the format is invalid.
        /// </summary>
        public static IBehaviorNode LoadTreeFromToken(JToken treeToken, BtContext context)
        {
            if (treeToken.Type == JTokenType.String)
            {
                var treeId = treeToken.ToString();
                return LoadTreeFromResources(treeId, context);
            }

            if (treeToken.Type != JTokenType.Object)
                throw new Exception(
                    $"[{ScriptName}] Invalid tree structure:  must be string (tree ID) or object (inline BT).");
        
            var obj = (JObject)treeToken;
            var rootToken = obj[BtJsonFields.Root] ?? obj;
            
            BtTreeBuilderExtension.ResolveRefs(rootToken, context);
            Debug.Assert(!BtTreeBuilderExtension.HasUnresolvedRefs(rootToken), $"[{ScriptName}] Unresolved {BtJsonFields.Ref} found post-resolution.");
            
            //BtProfileResolver.ResolveAllProfiles(rootToken as JObject, context);
            
            return Build(rootToken, context);

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
                throw new Exception($"[{ScriptName}] Missing or empty 'type'/'btKey' in node: {nodeData}");

            var factory = BtNodeRegistry.GetFactoryByAlias(alias);
            var node = factory.CreateNode(nodeData, context, recurse);

            // Wrap with lifecycle if needed
            if (node is ISystemCleanable)
                node = new BtLifecycleNode(node);

            return node;
        }
    
        /// <summary>
        /// Loads a behavior tree from Unity Resources using the specified tree ID.
        /// Expects a JSON file at Resources/Data/BTs/{treeId}.json.
        /// Resolves all references before building the tree.
        /// Throws if the file is missing or invalid.
        /// </summary>
        private static IBehaviorNode LoadTreeFromResources(string treeId, BtContext context)
        {
            var scriptName = nameof(BtTreeBuilder);
            var path = $"Data/BTs/{treeId}"; // [2025-07-05] TODO need to fix this path(?) Most likely handle the path at LoadTreeFromToken.  
            var textAsset = Resources.Load<TextAsset>(path);

            if (!textAsset)
                throw new Exception($"[{scriptName}] Could not load BT file at Resources/{path}.json");

            try
            {
                var fileJson = JObject.Parse(textAsset.text);
                var rootToken = fileJson[BtJsonFields.Root] ?? fileJson;

                BtTreeBuilderExtension.ResolveRefs(rootToken, context);
                Debug.Assert(!BtTreeBuilderExtension.HasUnresolvedRefs(rootToken), $"[{scriptName}] Unresolved {BtJsonFields.Ref} found post-resolution.");
            
                if (rootToken is not JObject rootNode)
                    throw new Exception($"[{scriptName}] Invalid tree structure: '{BtJsonFields.Root}' must be an object with a '{BtJsonFields.Type}' field.");

                if (string.IsNullOrEmpty(rootNode[BtJsonFields.Type]?.ToString()))
                    throw new Exception($"[{scriptName}] Missing or empty '{BtJsonFields.Type}' in root node.");
            
                return Build(rootNode, context);
            }
            catch (Exception ex)
            {
                throw new Exception($"[{scriptName}] Failed to parse behavior tree '{treeId}': {ex.Message}", ex);
            }
        }
    }
}