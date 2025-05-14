using UnityEngine;

/// <summary>
/// Represents the orchestrator responsible for constructing a full Blackboard
/// by coordinating multiple IContextBuilderModules.
///
/// Typically implemented by ModularContextBuilder.
///
/// ⚠️ NOTE: This interface owns the Blackboard lifecycle. ⚠️
/// It must create, populate, and return the final blackboard instance.
/// Modules used within this builder must conform to IContextBuilderModule.
/// </summary>
public interface IContextBuilder
{
    /// <summary>
    /// Creates and returns a fully-initialized blackboard for the provided entity.
    /// This method is responsible for coordinating all IContextBuilderModules.
    /// </summary>
    Blackboard Build(GameObject entity);
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