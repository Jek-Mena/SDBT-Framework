using UnityEngine;

public static class BtBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        // One call to load/caches all assets/configs
        GameAssets.Bootstrap();
        
        // Test 
        EntitySpawner.Spawn("enemy_standard_laghound");
        // End Test
        
        // Register all node factories
        // Initializes the Behavior Tree system by setting up default nodes,
        BtNodeRegistrationList.InitializeDefaults();
        
        // Register all node schemas
        // Initialize the blackboard builder and assign it to the context builder.
        // ---> Currently handled by the editor <---
        
        // Register all plugins
        RegisterPlugins();
        
        // Set up and register context builder and context modules
        var btBlackboardBuilder = new BtBlackboardBuilder();
        ContextModuleRegistration.RegisterAll(btBlackboardBuilder);
        
        // Assigns the blackboard builder instance to the Behavior Tree context,
        // enabling the construction and management of shared data across nodes.
        BtServices.ContextBuilder = btBlackboardBuilder;
        
        // (and register any others…)
        // BtNodeRegistry.AutoRegisterAllFactories();
    }

    private static void RegisterPlugins()
    {
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

        /*PluginMetadataStore.Register<NavMeshMoveToTargetPlugin>(
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
        );*/
    }
}

