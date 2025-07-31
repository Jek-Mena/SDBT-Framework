using AI.BehaviorTree.Nodes.Perception;
using AI.BehaviorTree.Registry.ContextBuilderModules;
using AI.BehaviorTree.Runtime.Context;
using AI.GroupAI;
using Systems.TargetingSystem;
using UnityEngine;

namespace AI.BehaviorTree.Registry.List
{
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
        public static void RegisterAll(BtContextBuilder builder)
        {
            Debug.Log("Registering all context modules...");
            var modules = new IContextBuilderModule[]
            {
                // [2025-06-18] IMPORTANT: ProfileContextBuilder MUST come before any context module
                // that depends on profiles (movement, targeting, etc.). If you add a new profile-dependent module,
                // always register it after the profile module.
            
                // Injects all profile dictionaries into the blackboard
                new ProfileBuilderModule(),
            
                // All systems that depend on profiles must come AFTER profile injection
                new UpdatePhaseExecutorBuilderModule(),
            
                new TargetingContextBuilder(),
                //new TargetingBuilderModule(),
                //new HealthBuilderModule(),
                
                new FormationBuilderModule(),
                new PerceptionBuilderModule(),
                //new StatBuilderModule(),
            
                new DebugOverlayBuilderModule()
            };

            foreach (var module in modules)
            {
                builder.RegisterModule(module);
            }
            Debug.Log("All context modules registered...");
        }
    }
}