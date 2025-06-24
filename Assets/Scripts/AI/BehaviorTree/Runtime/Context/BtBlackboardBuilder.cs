using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [2025-06-13] Refactored for explicit context module logging and fail-fast blackboard build.
/// - Logs registered modules and injection steps
/// - Dumps blackboard contents after build
/// - Throws if any required dependency missing
/// 
/// Constructs and injects runtime blackboards with modular services required by Behavior Tree plugins and nodes.
/// This serves as the main orchestrator for assembling per-entity context (e.g., movement logic, timers, targeting).
/// 
/// • Each context field is added by a separate IContextBuilderModule (e.g., MovementContextBuilder).
/// • ContextBuilder does *not* inject behavior, it only wires data and logic used by plugins/nodes.
/// • Modules are registered during startup (via BtBootStrapper) and executed in registration order.
///
/// - Supports future extensions such as Roslyn-generated modules for dynamic BT behaviors.
/// </summary>
public class BtBlackboardBuilder
{
    /// <summary>
    /// Registry of all service injectors. Each module encapsulates a single domain (e.g., movement, status effects).
    /// This enables separation of concerns, clean testing, and future runtime injection of new systems.
    /// </summary>
    private readonly List<IContextBuilderModule> _modules = new();

    /// <summary>
    /// Builds the full runtime context (blackboard) for an entity.
    /// 
    /// • Skips build if already processed (prevents redundant context initialization)
    /// • Ensures BtController exists (throws if missing)
    /// • Creates a fresh blackboard
    /// • Runs all registered modules to populate shared services
    /// • Assigns blackboard to BtController
    /// 
    /// ! Each module must be idempotent and safe (i.e., no hard assumptions about components).
    /// ! Will reuse existing context if already built for the entity.
    /// </summary>
    public BtContext BuildContext(GameObject agent)
    {
        // Retrieve and ensure the BtController component exists on the entity.
        var controller = agent.RequireComponent<BtController>();
        
        // Check if the blackboard has already been built for this entity.
        // If so, throw an exception to prevent redundant initialization.
        if (controller.Blackboard != null)
            throw new Exception($"[{nameof(BtBlackboardBuilder)}] Blackboard already set for {agent.name}! Double build is a bug.");
        
        var blackboard = new Blackboard();
        // Create a preliminary context with what you have
        var context = new BtContext(controller, blackboard, agent);

        Debug.Log($"[{nameof(BtBlackboardBuilder)}] Starting context build for '{agent.name}' using {_modules.Count} modules: " +
                  string.Join(", ", _modules.ConvertAll(m => m.GetType().Name)));

        foreach (var module in _modules)
        {
            Debug.Log($"[{nameof(BtBlackboardBuilder)}] -- Executing {module.GetType().Name}...");
            try
            {
                module.Build(context);
                Debug.Log($"[{nameof(BtBlackboardBuilder)}] ---- {module.GetType().Name} completed successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception($"[{nameof(BtBlackboardBuilder)}] Failed to build context for {agent.name}: {ex.Message}", ex);
            }
        }
        
        Debug.Log($"[{nameof(BtBlackboardBuilder)}] Blackboard built for '{agent.name}'. Dump:\n{blackboard.DumpContents()}");
            
        return context;
    }
    
    /// <summary>
    /// Registers a new module into the builder pipeline.
    /// </summary>
    public void RegisterModule(IContextBuilderModule module) => _modules.Add(module);
}