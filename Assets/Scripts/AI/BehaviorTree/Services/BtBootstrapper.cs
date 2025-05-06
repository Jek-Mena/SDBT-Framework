using UnityEngine;

public static class BtBootStrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        BtServices.ContextBuilder = new ContextBuilder();

        // 1) Must run BEFORE loading the tree so that NavMeshMover.Initialize(...) happens
        FluentPluginRegistry.Register<NavMeshMoverPlugin>(
            PluginPhase.Context
        );

        // 2) Then build the tree
        FluentPluginRegistry.Register<BtLoadTreePlugin>(
            PluginPhase.Context,
            typeof(NavMeshMoverPlugin)    // ensure MovementData is initialized first
        );

        // 3) Then apply runtime modifiers
        FluentPluginRegistry.Register<MovementModifierPlugin>(
            PluginPhase.Modifier,
            typeof(BtLoadTreePlugin)
        );

        // (and register any others…)
        BtNodeRegistry.AutoRegisterAllFactories();
    }
}

