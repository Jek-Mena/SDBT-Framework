using UnityEngine;

/// <summary>
/// [2025-06-13] Curates registration and grouping of context builder modules.
/// - Acts as a centralized registration point for context modules used in AI blackboard construction.
/// - Ensures that all eligible modules are systematically added to avoid missing dependencies or logic gaps.
/// - Simplifies maintenance by abstracting module registration away from the core blackboard building process.
/// This class primarily organizes and standardizes AI context initialization by collecting modules for subsequent injection
/// into runtime blackboards. Modules are assembled in a fail-fast and auditable manner to support debugging and future extension.
/// </summary>
public static class ContextModuleRegistrationList
{
    public static void RegisterAll(BtBlackboardBuilder builder)
    {
        Debug.Log("Registering all context modules...");
        var modules = new IContextBuilderModule[]
        {
            // [2025-06-18] IMPORTANT: ProfileContextBuilder MUST come before any context module
            // that depends on profiles (movement, targeting, etc.). If you add a new profile-dependent module,
            // always register it after the profile module.
            
            // Injects all profile dictionaries into blackboard
            new ProfileBuilderModule(),
            
            // All systems that depend on profiles must come AFTER profile injection
            new TimerBuilderModule(),
            new StatusEffectManagerBuilderModule(),
            new UpdatePhaseExecutorBuilderModule(),
            
            //new TargetingContextBuilderModule(),
            new HealthBuilderModule(),
            new MovementBuilderModule(),   // <-- ADD HERE, order matters if anything uses MovementLogic!
            new RotationBuilderModule(),   // <-- ADD HERE, order matters if anything uses RotationLogic!
            
            new PerceptionBuilderModule(),
            new StatBuilderModule(),
            
            new DebugOverlayBuilderModule()
        };

        foreach (var module in modules)
        {
            builder.RegisterModule(module);
        }
        Debug.Log("All context modules registered...");
    }
}