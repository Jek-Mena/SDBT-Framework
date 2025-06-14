/// <summary>
/// [2025-06-13] Pipeline Refactor: Context Builder Construction
/// 
/// - The AI context pipeline (all core context modules) is globally registered ONCE at startup by BtBootstrapper.
/// - All entity/NPC/AI blackboards are built by CLONING the globally-registered builder.
/// - Per-entity modules (such as BtConfigContextBuilderModule) are inserted as needed, after cloning.
/// - This ensures a single, auditable pipeline for all AI entities, with explicit, centralized module order.
/// - All other registration/initialization code has been removed from the pipeline (see ContextModuleRegistration and BtBootstrapper).
/// 
/// Pipeline Construction Flow (Text Version):
/// 
///     [App Startup]
///         -> BtBootstrapper
///             -> new BtBlackboardBuilder()
///             -> ContextModuleRegistration.RegisterAll(builder) // Add all core modules here, in order
///             -> BtServices.ContextBuilder = builder
///
///     NOTE: Entity Spawn will be refactored and clean in due time.
///     [Entity Spawn / AI Init]
///         -> ContextBuilderFactory.CreateWithBtConfig(config)
///             -> builder = BtServices.ContextBuilder.Clone()
///             -> builder.InsertModuleAtStart(new BtConfigContextBuilderModule(config)) // Per-entity config only
///             -> return builder
///         -> contextBuilder.Build(entity) // Build and inject blackboard/context for entity
/// 
/// No core module registration occurs anywhere else. The pipeline is explicit and maintainable.
/// </summary>
public static class ContextBuilderFactory
{
    public static IContextBuilder CreateWithBtConfig(ConfigData config)
    {
        if (BtServices.ContextBuilder is not BtBlackboardBuilder baseBuilder)
            throw new System.Exception("BtServices.ContextBuilder must be BtBlackboardBuilder.");
        
        var builder = baseBuilder.Clone();
        builder.InsertModuleAtStart(new BtConfigContextBuilderModule(config));
        return builder;
    }
}