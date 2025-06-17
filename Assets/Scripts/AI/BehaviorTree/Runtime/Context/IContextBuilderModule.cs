using UnityEngine;
/// <summary>
/// [ARCHITECTURE UPDATE -- 2025-06-17]
/// All context builder modules receive the full BtContext,
/// ensuring access to the Agent, Blackboard, Controller, and all other shared runtime systems.
/// This avoids fragmenting the pipeline between GameObject/Blackboard-only phases and full-context phases.
/// 
/// ⚠️ NOTE: Do not confuse this with IContextBuilder. ⚠️
/// This interface is for a single piece of logic (e.g., MovementContextBuilder),
/// not for building the entire blackboard.
/// </summary>
public interface IContextBuilderModule
{
    /// <summary>
    /// Populates part of the given blackboard for the provided entity.
    ///
    /// - Must NOT create a new Blackboard.
    /// - Called once per entity during BtLoader.ApplyAll().
    /// - All modules operate on the same shared Blackboard instance.
    /// </summary>
    void Build(BtContext context);
}

/*
   ┌────────────────────┐
   │   IContextBuilder  │  ← Owns full Blackboard creation
   └────────────────────┘
             │
             │ calls
             ▼
   ┌──────────────────────────────┐
   │  IContextBuilderModule[]     │  ← Injects parts of the blackboard
   │  e.g., Movement, Targeting   │
   └──────────────────────────────┘
             │
             ▼
   ┌────────────────────┐
   │     Blackboard     │  ← Shared runtime context
   └────────────────────┘

   Legend:
   - IContextBuilder → Coordinates everything
   - IContextBuilderModule → Only modifies a slice of the blackboard
   - Blackboard → Shared runtime memory per entity
*/