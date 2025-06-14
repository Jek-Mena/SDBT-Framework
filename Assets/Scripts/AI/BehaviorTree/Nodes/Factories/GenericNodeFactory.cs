using System;
using Newtonsoft.Json.Linq;

/// <summary>
/// [2025-06-14] New file
/// A generic factory class for creating behavior tree nodes. This factory is responsible for
/// interpreting the configuration JSON and mapping it to node-specific data structures
/// and behavior tree node instances.
/// </summary>
/// <typeparam name="TNode">
/// The type of the behavior node created by this factory. Must implement the <see cref="IBehaviorNode"/> interface.
/// </typeparam>
/// <typeparam name="TData">
/// The type of the data object extracted or built from the node configuration settings.
/// </typeparam>
public class GenericNodeFactory<TNode, TData> : IBtNodeFactory
    where TNode : IBehaviorNode
{
    private readonly Func<JObject, string, TData> _dataBuilder;
    private readonly Func<TData, BtContext, IBehaviorNode> _nodeCreator;

    public GenericNodeFactory(
        Func<JObject, string, TData> dataBuilder,
        Func<TData, BtContext, IBehaviorNode> nodeCreator)
    {
        _dataBuilder = dataBuilder;
        _nodeCreator = nodeCreator;
    }

    public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> _)
    {
        var factoryContext = typeof(TNode).Name + "Factory";
        var config = nodeData.Settings ?? throw new Exception($"[{factoryContext}] Missing config for node.");
        var data = _dataBuilder(config, factoryContext);
        return _nodeCreator(data, context);
    }
}

