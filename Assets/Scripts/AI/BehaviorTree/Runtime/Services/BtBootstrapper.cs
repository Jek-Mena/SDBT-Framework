using UnityEngine;

public static class BtBootStrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        BtNodeRegistry.InitializeDefaults();
        // BtNodeRegistry.PrintRegisteredNodes();

        // 🔧 Create modular builder and register context modules
        var modularContextBuilder = new ModularContextBuilder();

        modularContextBuilder.RegisterModule(
            new GenericBlackboardBinder<
                NavMeshMoveToTarget,
                MovementSettings,
                MovementSettingsModifierProvider>(
                providerKey: BlackboardKeys.Movement.ProviderKey,
                staticFieldBinder: (bb, comp) => bb.MovementLogic = comp
            ));
        
        modularContextBuilder.RegisterModule(new DefaultTargetContextBuilder());        // Defaults to "Player" tag
        modularContextBuilder.RegisterModule(new TimedExecutionContextBuilder());       // wires timer system

        BtServices.ContextBuilder = modularContextBuilder;

        // 1) Must run BEFORE loading the tree so that NavMeshMover.Initialize(...) happens
        PluginMetadataStore.Register<BtLoadTreePlugin>(
            pluginKey: BehaviorTreeKeys.Plugin.BtLoadTree,
            schemaKey: BehaviorTreeKeys.Schema.BtLoadTree,
            phase: PluginExecutionPhase.Context
        );

        PluginMetadataStore.Register<NavMeshMoveToTargetPlugin>(
            pluginKey: MovementKeys.Plugin.NavMeshMoveToTarget,
            schemaKey: MovementKeys.Schema.NavMeshMoveToTarget,
            phase: PluginExecutionPhase.Context
        );

        PluginMetadataStore.Register<PauseNodePlugin>(
            pluginKey: TimedExecutionKeys.Plugin.Pause,
            schemaKey: TimedExecutionKeys.Schema.Pause,
            phase: PluginExecutionPhase.TimedExecution
        );

        // (and register any others…)
        // BtNodeRegistry.AutoRegisterAllFactories();
    }
}

