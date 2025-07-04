/// <summary>
/// CoreKeys defines the universal JSON grammar shared across all
/// entity and behavior tree configurations. These keys are NOT
/// domain-specific � they are the structure that wraps all other keys.
/// 
/// Use these constants to access standard fields like "plugin",
/// "params", or "btKey" safely across your systems.
/// </summary>
public static class CoreKeys
{
    /// <summary>
    /// JSON field that maps to the unique identifier for an entity instance.
    /// 
    /// Example:
    /// {
    ///     "entityId": enemy_standard_chaser",
    ///     "components": [ ... ]
    /// }
    /// </summary>
    public const string EntityId = "entityId";
    
    /// <summary>
    /// JSON field that maps to the Unity prefab path used to instantiate the entity.
    /// 
    /// Example:
    /// {
    ///     "prefab": "Prefabs/Enemy/Standard/enemy_standard_chaser",
    ///     "components": [ ... ]
    /// }
    /// </summary>
    public const string Prefab = "prefab";

    public const string Components = "components";

    /// <summary>
    /// JSON field that maps to the plugin ID used for entity components.
    /// 
    /// Example:
    /// {
    ///     "plugin": "Plugin/NavMeshMoveToTarget",
    ///     "params": { ... }
    /// }
    /// </summary>
    public const string Plugin = "plugin";

    /// <summary>
    /// The configuration parameters passed to a plugin.
    /// Typically an object containing values for setup.
    /// 
    /// Example:
    /// {
    ///     "params": {
    ///         "speed": 3.0,
    ///         "target": "Player"
    ///     }
    /// }
    /// </summary>
    public const string Params = "params";

    /// <summary>
    /// Primary key used to identify the type of a behavior tree node in definitions.
    /// Synonymous with "btKey"; both are interchangeable.
    /// 
    /// Example:
    /// {
    ///     "type": "Bt/TimeoutDecorator"
    ///     // or
    ///     "btKey": "Bt/TimeoutDecorator"
    /// }
    /// </summary>
    public const string Type = "type";

    /// <summary>
    /// Configuration or Settings block passed to a behavior tree node.
    /// 
    /// Example:
    /// {
    ///     "type": "Bt/Pause",
    ///     "config": {
    ///         "duration": 3.0
    ///     }
    /// }
    /// </summary>
    public const string Config = "config";

    /// <summary>
    /// Defines the single child node for a decorator behavior tree node.
    /// This is used when the node modifies or wraps the behavior of exactly one child.
    /// 
    /// Example:
    /// {
    ///     "type": "Bt/Repeater",
    ///     "child": {
    ///         "type": "Bt/Wait",
    ///         "duration": 1.0
    ///     }
    /// }
    /// </summary>
    public const string Child = "child";

    /// <summary>
    /// Defines the child or children nodes for a composite behavior tree node.
    /// Can be an array or object depending on the node type.
    /// 
    /// Example:
    /// {
    ///     "type": "Bt/Sequence",
    ///     "children": [ ... ]
    /// }
    /// </summary>
    public const string Children = "children";

    /// <summary>
    /// Defines the unique identifier for a behavior tree entity configuration.
    /// Used as a lookup key when instantiating entities or referencing them in systems.
    /// 
    /// Example:
    /// {
    ///     "id": "enemy_standard_chaser",
    ///     ...
    /// }
    /// </summary>

    public const string BehaviorTree = "behaviorTree";

    /// <summary>
    /// Defines a reference to a shared config block within the behavior tree setup.
    /// Used to avoid duplicating parameters across multiple nodes by pointing to a named config inside a plugin (usually BtConfig).
    ///
    /// Example:
    /// {
    ///     "type": "Bt/MoveToTarget",
    ///     "config": { "$ref": "movement" }
    /// }
    /// </summary>
    public const string Ref = "$ref";

    /// <summary>
    /// Defines the entry point node of a behavior tree structure.
    /// All execution begins from this node when the tree is evaluated.
    /// 
    /// Example:
    /// {
    ///     "root": {
    ///         "type": "Bt/Repeater",
    ///         "children": {
    ///             "type": "MoveTo",
    ///             "config": {
    ///                 "overrideSpeed": 3.5
    ///             }
    ///         }
    ///     }
    /// }
    /// </summary>
    public const string Root = "root";

    
    public const string ExitCondition = "exitCondition";

    /// <summary>
    /// Represents the JSON field that specifies the domain or category context
    /// within which a particular entity or operation is applicable.
    /// </summary>
    public const string Domain = "Domain";

    /// <summary>
    /// The block for agent-global profiles (fear, health, etc.).
    /// </summary>
    public const string AgentProfiles = "agentProfiles";
    /// <summary>
    /// The block for behavior/BT node profiles (movement, targeting, timing, etc.).
    /// </summary>
    public const string BehaviorProfiles = "behaviorProfiles";
    
    public static class ResolvedProfiles
    {
        public const string Resolved = "resolved";
        public const string Targeting = Resolved + AgentConfigProfileBlocks.Targeting;
    }
}