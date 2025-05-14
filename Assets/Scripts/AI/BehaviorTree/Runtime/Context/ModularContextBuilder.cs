using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constructs and injects runtime blackboards with modular services required by Behavior Tree plugins and nodes.
/// This serves as the main orchestrator for assembling per-entity context (e.g., movement logic, timers, targeting).
/// 
/// • Each context field is added by a separate IContextBuilderModule (e.g., MovementContextBuilder).
/// • ContextBuilder does *not* inject behavior — it only wires data and logic used by plugins/nodes.
/// • Modules are registered during startup (via BtBootStrapper) and executed in registration order.
///
/// - Supports future extensions such as Roslyn-generated modules for dynamic BT behaviors.
/// </summary>
public class ModularContextBuilder : IContextBuilder
{
    /// <summary>
    /// Registry of all service injectors. Each module encapsulates a single domain (e.g., movement, status effects).
    /// This enables separation of concerns, clean testing, and future runtime injection of new systems.
    /// </summary>
    private readonly List<IContextBuilderModule> _modules = new();

    /// <summary>
    /// Tracks entities that have already had their context built.
    ///
    /// • Prevents duplicate initialization of BtController blackboards
    /// • Ensures Build(...) is idempotent at the entity level
    /// </summary>
    private readonly HashSet<GameObject> _built = new();

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
    public Blackboard Build(GameObject entity)
    {
        if (_built.Contains(entity))
        {
            Debug.LogWarning($"[ModularContextBuilder] Skipping duplicate context build for '{entity.name}'");
            return entity.RequireComponent<BtController>().Blackboard;
        }

        var controller = entity.RequireComponent<BtController>();
        var blackboard = new Blackboard();

        foreach (var module in _modules)
            module.Build(entity, blackboard);
            
        controller.InitContext(blackboard);
        
        Debug.Log($"[ModularContextBuilder] Context built for '{entity.name}' using {_modules.Count} modules.");
        return blackboard;
    }

    /// <summary>
    /// Registers a new module into the builder pipeline.
    /// 
    /// • Must be called during startup (e.g., inside BtBootStrapper)
    /// • Call order matters if modules depend on each other
    /// </summary>
    public void RegisterModule(IContextBuilderModule module)
    {
        _modules.Add(module);
    }
}