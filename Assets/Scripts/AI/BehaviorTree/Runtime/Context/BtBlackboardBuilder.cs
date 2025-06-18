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
public class BtBlackboardBuilder : IContextBuilder
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
    public Blackboard Build(GameObject agent)
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
        
        controller.InitContext(context);
        Debug.Log($"[{nameof(BtBlackboardBuilder)}] Blackboard built for '{agent.name}'. Dump:\n{blackboard.DumpContents()}");
            
        return blackboard;
    }
    
    /// <summary>
    /// Creates a duplicate instance of the current <see cref="BtBlackboardBuilder"/>.
    /// • Populates the clone with all registered context builder modules from the original instance.
    /// • Modules in the clone are safe to reuse since they are assumed to be stateless.
    /// This allows creating separate builder instances while preserving configuration from the original instance.
    /// </summary>
    /// <returns>A new instance of <see cref="BtBlackboardBuilder"/> containing the same modules as the original.</returns>
    public IContextBuilder Clone()
    {
        var clone = new BtBlackboardBuilder();
        foreach (var module in _modules)
            clone.RegisterModule(module); // stateless, safe to reuse
        return clone;
    }

    /// <summary>
    /// Registers a new module into the builder pipeline.
    /// 
    /// • Must be called during startup (e.g., inside BtBootStrapper)
    /// • Call order matters if modules depend on each other
    /// </summary>
    public void RegisterModule(IContextBuilderModule module) => _modules.Add(module);

    /// <summary>
    /// Inserts the specified module at the beginning of the module list.
    /// Ensures that the module gets executed before all other modules during the build process.
    /// </summary>
    /// <param name="module">The builder module to be added at the start of the module list.</param>
    public void InsertModuleAtStart(IContextBuilderModule module) => _modules.Insert(0, module);
    
}