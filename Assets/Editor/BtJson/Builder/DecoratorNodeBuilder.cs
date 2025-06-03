using Newtonsoft.Json.Linq;

public class DecoratorNodeBuilder
{
    /// <summary>
    /// Creates a JSON object representing a Repeater decorator node.
    /// The Repeater node allows repeating its child nodes either a specified number of times or indefinitely.
    /// </summary>
    /// <param name="maxRepeat">
    /// The maximum number of repetitions. If set to -1, the node will repeat indefinitely.
    /// </param>
    /// <param name="children">
    /// An array of child nodes to be included under the Repeater node.
    /// </param>
    /// <returns>
    /// A JObject representing the Repeater decorator node with its configuration and children.
    /// </returns>
    public JObject Repeater(int maxRepeat = -1, params object[] children) =>
        new JObject
        {
            [CoreKeys.Type] = BtNodeTypes.Decorators.Repeater,
            [CoreKeys.Config] = new JObject { [BtNodeFields.Repeater.MaxRepeats] = maxRepeat },
            [CoreKeys.Children] = new JArray(children)
        };

    /// <summary>
    /// Creates a JSON object representing a Timeout decorator node.
    /// The Timeout node enforces a maximum execution duration on its child nodes.
    /// </summary>
    /// <param name="data">
    /// A TimedExecutionData object containing the label and duration configuration for the Timeout decorator node.
    /// </param>
    /// <returns>
    /// A JObject representing the Timeout decorator node with its configuration and duration.
    /// </returns>
    public JObject Timeout(TimedExecutionData data)
    {
        return new JObject
        {
            [CoreKeys.Type] = BtNodeTypes.Decorators.Timeout,
            [CoreKeys.Config] = new JObject
            {
                [BtConfigFields.Common.Label] = data.Label,
                [BtConfigFields.Common.Duration] = new JObject
                {
                    [CoreKeys.Ref] = data.Duration
                }
            }
        };
    }
}