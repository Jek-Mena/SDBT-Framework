using System.Collections.Generic;
using UnityEngine;

/// <summary>
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
    public Blackboard Build(GameObject entity)
    {
        // Retrieve and ensure the BtController component exists on the entity.
        var controller = entity.RequireComponent<BtController>();
        
        // Check if the blackboard has already been built for this entity.
        // If so, throw an exception to prevent redundant initialization.
        if (controller.Blackboard != null)
            throw new System.Exception($"[BtBlackboardBuilder] Blackboard already set for {entity.name}! Double build is a bug.");
        var blackboard = new Blackboard();

        // Retrieve and ensure the TimerExecutionMono component exists on the entity; throws exception if none.
        // Assign the TimerExecutionMono instance to the blackboard for shared use.
        var timeExecutionManager = entity.RequireComponent<TimerExecutionMono>();
        blackboard.TimeExecutionManager = timeExecutionManager;

        // Ensure the StatusEffectManager component exists on the entity; throws exception if none.
        // Assign the StatusEffectManager to the blackboard.
        var statusEffectManager = entity.RequireComponent<StatusEffectManager>();
        blackboard.StatusEffectManager = statusEffectManager;

        // Retrieve and ensure the UpdatePhaseExecutor component exists on the entity; throws an exception if none.
        // Assign the UpdatePhaseExecutor instance to the blackboard for shared use.
        var updatePhaseExecutor = entity.RequireComponent<UpdatePhaseExecutor>();
        blackboard.UpdatePhaseExecutor = updatePhaseExecutor;
        
        // Initialize the context for the BtController with the newly created blackboard.
        // Log the successful creation and initialization of the blackboard, including the number of modules used.
        controller.InitContext(blackboard);
        Debug.Log($"[BtBlackboardBuilder] Blackboard created and context initialize for '{entity.name}' using {_modules.Count} modules.");

        //
        foreach (var module in _modules)
            module.Build(entity, blackboard);
            
        return blackboard;
    }

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
    /// � Must be called during startup (e.g., inside BtBootStrapper)
    /// � Call order matters if modules depend on each other
    /// </summary>
    public void RegisterModule(IContextBuilderModule module) => _modules.Add(module);
    
    public void InsertModuleAtStart(IContextBuilderModule module) => _modules.Insert(0, module);
    
}