using Newtonsoft.Json.Linq;

/// <summary>
/// Represents a builder for creating composite nodes in a behavior tree.
/// Composite nodes are used to structure the behavior tree by organizing and controlling
/// the execution of their child nodes.
/// </summary>
public class CompositeNodeBuilder
{
    /// <summary>
    /// Constructs a JSON object representing a Sequence composite node with its children.
    /// A Sequence processes its children in order and succeeds only if all children succeed.
    /// </summary>
    /// <param name="children">
    /// The child nodes to be included within the Sequence composite node.
    /// </param>
    /// <returns>
    /// A JSON object containing the type and child nodes for the Sequence composite.
    /// </returns>
    public JObject Sequence(params object[] children) =>
        new JObject
        {
            [CoreKeys.Type] = BtNodeTypes.Composite.Sequence,
            [CoreKeys.Children] = new JArray(children)
        };
    
    /// <summary>
    /// Constructs a JSON object representing a Selector composite node with its children.
    /// A Selector evaluates its children from left to right and succeeds if any child succeeds.
    /// </summary>
    /// <param name="children">
    /// The child nodes to be included within the Selector composite node.
    /// </param>
    /// <returns>
    /// A JSON object containing the type and child nodes for the Selector composite.
    /// </returns>
    public JObject Selector(params object[] children) =>
        new JObject
        {
            [CoreKeys.Type] = BtNodeTypes.Composite.Selector,
            [CoreKeys.Children] = new JArray(children)
        };

    /// <summary>
    /// Constructs a JSON object representing a Parallel composite node with its children.
    /// A Parallel node executes its children concurrently. The exit condition dictates when the node should stop execution.
    /// </summary>
    /// <param name="exitCondition">
    /// The condition that determines when the Parallel node should stop execution (e.g., when a specific child succeeds or fails).
    /// </param>
    /// <param name="children">
    /// The child nodes to be included within the Parallel composite node.
    /// </param>
    /// <returns>
    /// A JSON object containing the type, configuration (including exit condition), and child nodes for the Parallel composite.
    /// </returns>
    public JObject Parallel(string exitCondition, params object[] children) =>
        new JObject
        {
            [CoreKeys.Type] = BtNodeTypes.Composite.Parallel,
            [CoreKeys.Config] = new JObject { [CoreKeys.ExitCondition] = exitCondition },
            [CoreKeys.Children] = new JArray(children)
        };
    
}