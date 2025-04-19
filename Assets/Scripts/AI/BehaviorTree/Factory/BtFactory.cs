using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Debug = UnityEngine.Debug;

public static class BtFactory
{
    private static readonly Dictionary<string, IBehaviorNodeFactory> Factories = new();

    public static void Register(string key, IBehaviorNodeFactory factory)
    {
        Factories[key] = factory;
    }

    public static IBehaviorNode BuildTree(JToken json, Blackboard bb)
    {
        var obj = (JObject)json;
        var type = obj[JsonKeys.BehaviorTree.NodeType]?.ToString();
        if (type == null || !Factories.TryGetValue(type, out var factory))
            throw new Exception($"Unknown behavior node type: {type}");

        return factory.CreateNode(obj, bb);
    }

    public static void AutoRegisterAllFactories()
    {
        var attrType = typeof(BehaviorNodeAttribute);
        var factoryType = typeof(IBehaviorNodeFactory);
        var timedBaseType = typeof(TimedExecutionNode);

        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(a => a.GetTypes())
                     .Where(t => !t.IsAbstract))
        {
            var attr = type.GetCustomAttribute<BehaviorNodeAttribute>();
            if (attr == null) continue;

            if (factoryType.IsAssignableFrom(type))
            {
                // It is a normal node factory
                var factory = (IBehaviorNodeFactory)Activator.CreateInstance(type);
                Register(attr.NodeType, factory);
                Debug.Log($"[BT] Registered factory node: {attr.NodeType} via {type.Name}");
            }
            else if (timedBaseType.IsAssignableFrom(type))
            {
                // It is a TimedExecutionNode, wrap in generic factory
                var factoryTypeGeneric = typeof(TimedExecutionNodeFactory<>).MakeGenericType(type);
                var factory = (IBehaviorNodeFactory)Activator.CreateInstance(factoryTypeGeneric, attr.NodeType);
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

[AttributeUsage(AttributeTargets.Class)]
public class BehaviorNodeAttribute : Attribute
{
    public string NodeType { get; }

    public BehaviorNodeAttribute(string nodeType)
    {
        NodeType = nodeType;
    }
}

[BehaviorNode(NodeName.MoveTo)]
public class MoveNodeFactory : IBehaviorNodeFactory
{
    public IBehaviorNode CreateNode(JObject config, Blackboard blackboard)
    {
        return new MoveNode();
    }
}

public class TimedExecutionNodeFactory<T> : IBehaviorNodeFactory where T : TimedExecutionNode, new()
{
    private readonly string _nodeType;

    public TimedExecutionNodeFactory(string nodeType)
    {
        _nodeType = nodeType;
    }

    public IBehaviorNode CreateNode(JObject config, Blackboard blackboard)
    {
        var data = new TimedExecutionData()
        {
            key = $"{_nodeType}:{blackboard.GetHashCode()}",
            duration = config.Value<float?>(JsonKeys.TimedExecution.Duration) ?? 1f,
            startDelay = config.Value<float?>(JsonKeys.TimedExecution.StartDelay) ?? 0f,
            interruptible = config.Value<bool?>(JsonKeys.TimedExecution.Interruptible) ?? true,
            failOnInterrupt = config.Value<bool?>(JsonKeys.TimedExecution.FailOnInterrupt) ?? true,
            resetOnExit = config.Value<bool?>(JsonKeys.TimedExecution.ResetOnExit) ?? true,
            mode = config.Value<string>(JsonKeys.TimedExecution.Mode) switch
            {
                "Loop" => TimerExecutionMode.Loop,
                "UntilSuccess" => TimerExecutionMode.UntilSuccess,
                "UntilFailure" => TimerExecutionMode.UntilFailure,
                _ => TimerExecutionMode.Normal
            }
        };

        blackboard.CurrentNodeData = data;
        return new T();
    }
}

[BehaviorNode(NodeName.Pause)]
public class PauseNode : TimedExecutionNode
{
    // No additional logic unless you want to override Tick()
    // Could inject sound, animation trigger, etc.
}