using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Debug = UnityEngine.Debug;

public static class BtNodeRegistry
{
    private static readonly Dictionary<string, IBtNodeFactory> Factories = new();

    // ?? Public access to all registered node types
    public static IEnumerable<string> RegisteredNodeTypes => Factories.Keys;

    public static void Register(string key, IBtNodeFactory factory)
    {
        Factories[key] = factory;
    }

    public static IBehaviorNode BuildTree(JToken json, Blackboard bb)
    {
        return BuildTree(json, bb, (jt) => BuildTree(jt, bb)); // <= Recursive delegate
    }

    public static IBehaviorNode BuildTree(JToken json, Blackboard bb, Func<JToken, IBehaviorNode> recurse)
    {
        var obj = (JObject)json;
        var type = obj[JsonFields.Type]?.ToString();

        if (string.IsNullOrEmpty(type))
            throw new Exception($"[BT Loader] Missing or empty 'type' field in node: {obj}");

        if (type == null || !Factories.TryGetValue(type, out var factory))
            throw new Exception($"Unknown behavior node type: '{type}' (BT JSON: {obj})");
          
        return factory.CreateNode(obj, bb, recurse);
    }

    public static void AutoRegisterAllFactories()
    {
        var attrType = typeof(BtNodeAttribute);
        var factoryType = typeof(IBtNodeFactory);
        var timedBaseType = typeof(TimedExecutionNode);

        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(a => a.GetTypes())
                     .Where(t => !t.IsAbstract))
        {
            var attr = type.GetCustomAttribute<BtNodeAttribute>();
            if (attr == null) continue;

            if (factoryType.IsAssignableFrom(type))
            {
                // It is a normal node factory
                var factory = (IBtNodeFactory)Activator.CreateInstance(type);
                Register(attr.NodeType, factory);
                Debug.Log($"[BT] Registered factory node: {attr.NodeType} via {type.Name}");
            }
            else if (timedBaseType.IsAssignableFrom(type))
            {
                // It is a TimedExecutionNode, wrap in generic factory
                var factoryTypeGeneric = typeof(TimedExecutionNodeFactory<>).MakeGenericType(type);
                var factory = (IBtNodeFactory)Activator.CreateInstance(factoryTypeGeneric, attr.NodeType);
                Register(attr.NodeType, factory);
                Debug.Log($"[BT] Registered timed node: {attr.NodeType} via {factoryTypeGeneric.Name}");
            }
            else
            {
                Debug.LogWarning($"[BT] Ignored {type.Name} with [BehaviorNode] — not a factory or timed node.");
            }
        }
    }
    // NOTE On IL2CPP platforms (e.g., iOS), Unity may strip unused types, so you might need:
    // [UnityEngine.Scripting.Preserve]
}