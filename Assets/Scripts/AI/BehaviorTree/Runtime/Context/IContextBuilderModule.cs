using UnityEngine;

/// <summary>
/// Represents a single domain-specific context injector (e.g., movement, timer, targeting).
///
/// Each implementation should:
/// - Be stateless and reusable
/// - Only modify its own part of the blackboard
/// - Handle missing components gracefully
/// - Avoid side effects or assumptions about plugin order
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
    void Build(GameObject entity, Blackboard blackboard);
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