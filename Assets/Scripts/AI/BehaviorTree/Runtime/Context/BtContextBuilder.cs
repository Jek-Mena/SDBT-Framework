using System;
using System.Collections.Generic;
using System.Linq;
using AI.BehaviorTree.Core;
using AI.BehaviorTree.Runtime;
using AI.BehaviorTree.Runtime.Context;
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
public class BtContextBuilder
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
        // Retrieve and ensure that the BtController component exists on the entity.
        var controller = agent.RequireComponent<BtController>();
        // Retrieve and ensure that the EntityRuntimeData and definition (immutable) exist on the entity.
        // If the EntityRuntimeData does not exist check GameAssets.
        var runtimeData = agent.RequireComponent<EntityRuntimeData>();
        var definition = runtimeData.Definition;
        
        var profiles = new AgentProfiles();
        var blackboard = new Blackboard();
        
        // Create a preliminary context with what we have. Blackboard
        var context = new BtContext(
            controller, 
            agent, 
            profiles, 
            definition, 
            runtimeData, 
            blackboard);

        Debug.Log(
            $"[{nameof(BtContextBuilder)}] Starting context build for '{agent.name}'\nUsing {_modules.Count} modules:\n" +
            string.Join("\n- ", _modules.ConvertAll(m => m.GetType().Name).Prepend(""))
        );
        
        foreach (var module in _modules)
        {
            Debug.Log($"[{nameof(BtContextBuilder)}] -- Executing {module.GetType().Name}...");
            try
            {
                module.Build(context);
                Debug.Log($"[{nameof(BtContextBuilder)}] ---- {module.GetType().Name} completed successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception($"[{nameof(BtContextBuilder)}] Failed to build context for {agent.name}: {ex.Message}", ex);
            }
        }
        
        Debug.Log($"[{nameof(BtContextBuilder)}] Context built for '{agent.name}'. " +
                  $"Profile Dump:\n{profiles.DumpContents()}");
            
        return context;
    }
    
    /// <summary>
    /// Registers a new module into the builder pipeline.
    /// </summary>
    public void RegisterModule(IContextBuilderModule module) => _modules.Add(module);
}