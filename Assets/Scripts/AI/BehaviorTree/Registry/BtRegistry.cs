using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class BtRegistry
{
    private static readonly Dictionary<string, IBehaviorNode> Trees = new();
    /// <summary>
    /// Register a new behavior tree root node under a given key.
    /// </summary>
    public static void Register(string key, IBehaviorNode rootNode)
    {
        if (string.IsNullOrWhiteSpace(key) || rootNode == null)
        {
            Debug.LogError($"[BehaviorTreeRegistry] Attempted to register null key or node. Key: '{key}'");
            return;
        }
        Trees[key] = rootNode;
        Debug.Log($"[BehaviorTreeRegistry] Registered tree '{key}'");
    }
    
    /// <summary>
    /// Retrieve a tree root node by key.
    /// </summary>
    public static IBehaviorNode Get(string key)
    {
        if (Trees.TryGetValue(key, out var node))
            return node;
        Debug.LogError($"[BehaviorTreeRegistry] Tree key not found: '{key}'. Did you register it?");
        return null;
    }
    
    /// <summary>
    /// Utility for tests/debug: clear the registry (for test teardown, not for game usage).
    /// </summary>
    public static void Clear()
    {
        Trees.Clear();
    }
}

public static class BtRegistrationList
{
    private static bool _hasInitialized;
    
    public static void Initialize()
    {
        if (_hasInitialized) return;
        _hasInitialized = true;

        var configFolder = "Data/BTs"; // Resources path
        var textAssets = Resources.LoadAll<TextAsset>(configFolder);

        foreach (var asset in textAssets)
        {
            try
            {
                var rootNode = BtTreeBuilder.BuildFromJson(asset.text);
                var key = Path.GetFileNameWithoutExtension(asset.name);
                BtRegistry.Register(key, rootNode);
                Debug.Log($"[BehaviorTreeRegistrationList] Registered BT '{key}' from '{asset.name}'");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BehaviorTreeRegistrationList] Failed to register BT '{asset.name}': {ex.Message}");
            }
        }
    }
}