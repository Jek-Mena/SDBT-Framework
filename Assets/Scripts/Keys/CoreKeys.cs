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

    public const string Tree = "tree";

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
    /// Defines common config block keys used to structure reusable
    /// parameters inside Plugin/BtConfig. These keys are not type-specific,
    /// but serve as namespaces for grouped behavior data.
    ///
    /// Example:
    /// {
    ///     "plugins": [
    ///         {
    ///             "type": "Plugin/BtConfig",
    ///             "params": {
    ///                 "movement": {
    ///                     "speed": 3.0,
    ///                     "acceleration": 4.0
    ///                 },
    ///                 "dash": {
    ///                     "magnitude": 25.0
    ///                 }
    ///             }
    ///         }
    ///     ],
    ///     "btTree": {
    ///         "type": "Bt/MoveTo",
    ///         "config": { "$ref": "movement" }
    ///     }
    /// }
    /// </summary>
    public static class ParamSections
    {
        /// <summary>
        /// Shared configuration block name for all movement-related parameters.
        /// Only contains physical movement settings, no targeting logic.
        /// Used by BT nodes (e.g., MoveTo), movement plugins (e.g., NavMeshMoveToTarget), and any runtime
        /// system that requires movement tuning.
        /// 
        /// Config block structure:
        /// {
        ///     "speed": 3.5,
        ///     "acceleration": 4.0,
        ///     "angularSpeed": 120.0,
        ///     "stoppingDistance": 1.2,
        ///     "updateThreshold": 0.25,
        ///     ...
        /// }
        /// 
        /// Do not add targeting keys here�keep all target/tag logic in a separate "targeting" config.
        /// </summary>
        public const string Movement = "movement";

        /// <summary>
        /// Represents the JSON field key for the timing settings block (Temporal Condition).
        /// This key is used to identify and map timing-related configurations in the JSON data structure.
        /// </summary>
        public const string Timing = "timing";

        /// <summary>
        /// JSON field that corresponds to the setting for the rotation component within a configuration block.
        /// This can be used to define or modify rotation behavior in relevant systems.
        /// </summary>
        public const string Rotation = "rotation";
        
        /// <summary>
        /// Shared configuration block name for all targeting parameters.
        /// Purely defines how an entity selects its target�never how it moves toward it.
        /// Used by BT nodes (e.g., MoveTo, AttackTarget), targeting plugins, and runtime AI logic.
        /// 
        /// Config block structure:
        /// {
        ///     "targetTag": "Player",
        ///     "style": "Closest",    // TargetingStyle enum (Closest, Farthest, LowestHP, etc.)
        ///     "maxRange": 100,
        ///     "allowNull": false,
        ///     // Optional: custom criteria, exclusions, etc. TODO: To be implemented if needed
        /// }
        /// 
        /// Never mix movement keys here.
        /// Targeting config should be referenced via $ref or directly wherever selection logic is needed.
        /// </summary>
        public const string Targeting = "targetingProfiles";
    }
}