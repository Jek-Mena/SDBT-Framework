using Newtonsoft.Json.Linq;
using System.Linq;

/// <summary>
/// Strongly-typed wrapper for a serialized behavior tree node.
/// Encapsulates the node's type key, configuration, and children.
/// All node factories and BT builders should use this instead of raw JObject.
/// </summary>
public class TreeNodeData
{
    /// <summary>
    /// Teh raw JSON object representing the entire node (including type, config, children, etc.)
    /// </summary>
    public JObject Raw { get; }

    /// <summary>
    /// The node's type or alias key (e.g. "Bt/MoveTo", "Bt/Selector").
    /// </summary>
    public string BtKey => Raw[CoreKeys.BtKey]?.ToString() ?? Raw[CoreKeys.Type]?.ToString();

    /// <summary>
    /// The configuration/settings object for this node (e.g. parameters, options).
    /// Returns null if not present.
    /// </summary>
    public JObject Config => Raw[CoreKeys.Config] as JObject;

    /// <summary>
    /// The list of child nodes (composite, decorators).
    /// Returns null if not present.
    /// </summary>
    public JArray Children => Raw[CoreKeys.Children] as JArray;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="raw"></param>
    public TreeNodeData(JObject raw) => Raw = raw ?? throw new System.ArgumentNullException(nameof(raw));

    /// <summary>
    /// Returns true if this node has child nodes.
    /// </summary>
    public bool HasChildren => Children != null && Children.Count > 0;

    /// <summary>
    /// Returns a readable summary for debugging/
    /// </summary>
    public override string ToString()
    {
        return $"[TreeNodeData: BtKey={BtKey}, ConfigKeys={Config?.Properties()?.Count() ?? 0}, Children={Children?.Count ?? 0}]";
    }
}