/// <summary>
/// PluginMetaKeys centralizes the plugin + schema key pairings for all registered plugins.
/// This ensures fail-fast behavior, reduces duplication, and simplifies future tooling support.
///
/// Example usage:
/// PluginMetadataStore.RegisterSchema<NavMeshMoveToTargetPlugin>(
///     PluginMetaKeys.Movement.NavMeshMoveToTarget.Plugin,
///     PluginMetaKeys.Movement.NavMeshMoveToTarget.Schema,
///     PluginExecutionPhase.Configurable
/// );
/// </summary>
public static class PluginMetaKeys
{
    /// <summary>
    /// Contains core constants and nested classes that define metadata keys used for plugin configurations 
    /// and behavior tree operations within the game.
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// BtLoadTree is a core plugin that provides functionality for loading behavior trees.
        /// It defines constants for the plugin and schema keys, which are used to register
        /// and reference this plugin within the system.
        /// 
        /// Example usage found at BtBootstrapper:
        /// PluginMetadataStore.RegisterSchema(
        ///     PluginMetaKeys.Core.BtLoadTree.Plugin,
        ///     PluginMetaKeys.Core.BtLoadTree.Schema,
        ///     PluginExecutionPhase.Initialization
        /// );
        /// 
        /// This ensures consistent key usage and simplifies the integration of behavior tree
        /// loading functionality into the application.
        /// </summary>
        public static class BtLoadTree
        {
            public const string Plugin = "Plugin/BtLoadTree";
            public const string Schema = "Schema/BtLoadTree";
        }

        /// <summary>
        /// BtConfig is a shared configuration plugin that injects modular parameters (like timing, movement, targeting) into the blackboard under a centralized key.
        /// It acts as a cross-cutting data source for other plugins and behavior nodes via $ref or direct access.
        /// This enables clean separation of logic and data, making behavior trees more composable and maintainable.
        ///
        /// First example usage:
        /// {
        ///   "plugin": "Plugin/BtConfig", --- This part
        ///   "params": {3256
        ///     "timing": {
        ///       "moveDuration": 5.0,
        ///       "pauseDuration": 2.0
        ///     },
        ///     "movement": {
        ///       "speed": 3.5
        ///     }
        ///   }
        /// }
        ///
        /// Second example usage:
        /// controller.Blackboard.Set(PluginMetaKeys.Core.BtConfig.Plugin, config);
        /// 
        /// </summary>
        /// <remarks>
        /// This class provides constants that are used as keys for storing and retrieving
        /// behavior tree-related configuration data in the blackboard.
        /// </remarks>
        public static class BtConfig
        {
            public const string Plugin = "Plugin/BtConfig";
            public const string Schema = "Schema/BtConfig";
        }
    }

    public static class Movement
    {
        public static class NavMeshMoveToTarget
        {
            public const string Plugin = "Plugin/NavMeshMoveToTarget";
            public const string Schema = "Schema/NavMeshMoveToTarget";
        }

        public static class ImpulseRigidbodyMover
        {
            public const string Plugin = "Plugin/ImpulseRigidbodyMover";
            public const string Schema = "Schema/ImpulseRigidbodyMover";
        }
    }

    public static class Rotation
    {
        public static class TransformLookAtTarget
        {
            public const string Plugin = "Plugin/TransformLookAtTarget";
            public const string Schema = "Schema/TransformLookAtTarget";
        }
    }
    
    public static class Status
    {
        public static class Pause
        {
            public const string Plugin = "Plugin/Pause";
            public const string Schema = "Schema/Pause";
        }
    }

    public static class Targeting
    {
        public const string Plugin = "Plugin/Targeting";
        public const string Schema = "Schema/Targeting";
    }

    public static class TimedExecution
    {
        public static class Pause
        {
            public const string Plugin = "Plugin/Pause";
            public const string Schema = "Schema/Pause";
        }
    }
}