using UnityEngine;

public static class BtBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        // Initializes the Behavior Tree system by setting up default nodes,
        BtNodeRegistrationList.InitializeDefaults();

        // Initialize the blackboard builder and assign it to the context builder.
        var btBlackboardBuilder = new BtBlackboardBuilder();
        
        // Assigns the blackboard builder instance to the Behavior Tree context,
        // enabling the construction and management of shared data across nodes.
        BtServices.ContextBuilder = btBlackboardBuilder;

        PluginMetadataStore.Register<ConfigPlugin>(
            pluginKey: PluginMetaKeys.Core.BtConfig.Plugin,
            schemaKey: PluginMetaKeys.Core.BtConfig.Schema,
            phase: PluginExecutionPhase.Context
        );

        PluginMetadataStore.Register<BtLoadTreePlugin>(
            pluginKey: PluginMetaKeys.Core.BtLoadTree.Plugin,
            schemaKey: PluginMetaKeys.Core.BtLoadTree.Schema,
            phase: PluginExecutionPhase.BtExecution
        );

        PluginMetadataStore.Register<NavMeshMoveToTargetPlugin>(
            pluginKey: PluginMetaKeys.Movement.NavMeshMoveToTarget.Plugin,
            schemaKey: PluginMetaKeys.Movement.NavMeshMoveToTarget.Schema,
            phase: PluginExecutionPhase.Context
        );

        PluginMetadataStore.Register<QuaternionLookAtPlugin>(
            pluginKey: PluginMetaKeys.Rotation.TransformLookAtTarget.Plugin,
            schemaKey: PluginMetaKeys.Rotation.TransformLookAtTarget.Schema,
            phase: PluginExecutionPhase.Context
        );
        
        PluginMetadataStore.Register<TargetingPlugin>(
            pluginKey: PluginMetaKeys.Targeting.Plugin,
            schemaKey: PluginMetaKeys.Targeting.Schema,
            phase: PluginExecutionPhase.Context
        );

        PluginMetadataStore.Register<PauseNodePlugin>(
            pluginKey: PluginMetaKeys.TimedExecution.Pause.Plugin,
            schemaKey: PluginMetaKeys.TimedExecution.Pause.Schema,
            phase: PluginExecutionPhase.TimedExecution
        );
        

        // (and register any others…)
        // BtNodeRegistry.AutoRegisterAllFactories();
    }
}

