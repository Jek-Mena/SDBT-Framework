/// <summary>
/// [2025-06-13] Curates registration and grouping of context builder modules.
/// - Acts as a centralized registration point for context modules used in AI blackboard construction.
/// - Ensures that all eligible modules are systematically added to avoid missing dependencies or logic gaps.
/// - Simplifies maintenance by abstracting module registration away from the core blackboard building process.
/// This class primarily organizes and standardizes AI context initialization by collecting modules for subsequent injection
/// into runtime blackboards. Modules are assembled in a fail-fast and auditable manner to support debugging and future extension.
/// </summary>
public static class ContextModuleRegistration
{
    public static void RegisterAll(BtBlackboardBuilder builder)
    {
        var modules = new IContextBuilderModule[]
        {
            new CoreContextModules.TimerContextBuilder(),
            new CoreContextModules.StatusEffectContextBuilder(),
            new CoreContextModules.UpdatePhaseExecutorContextBuilder(),
            
            new MovementContextBuilderModule(),   // <-- ADD HERE, order matters if anything uses MovementLogic!
            new RotationContextBuilderModule(),   // <-- ADD HERE, order matters if anything uses RotationLogic!
            new TargetingContextBuilerModule(),
        };

        foreach (var module in modules)
        {
            builder.RegisterModule(module);
        }
    }
}