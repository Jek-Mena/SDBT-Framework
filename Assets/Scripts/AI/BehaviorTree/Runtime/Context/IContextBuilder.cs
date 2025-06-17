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
    /// Builds and returns a fully-initialized blackboard for the provided GameObject entity.
    /// This method should run all registered IContextBuilderModules in order.
    /// </summary>
    Blackboard Build(GameObject agent);

    /// <summary>
    /// Registers a context builder module to run at the end of the build pipeline.
    /// Modules are executed in registration order unless reordered.
    /// </summary>
    void RegisterModule(IContextBuilderModule module);

    /// <summary>
    /// Inserts a module at the beginning of the build pipeline.
    /// Used for core dependencies like config injection that must happen early.
    /// </summary>
    void InsertModuleAtStart(IContextBuilderModule module);

    /// <summary>
    /// Creates a deep copy of the builder and its registered modules.
    /// Used to fork context builders for per-entity customization.
    /// </summary>
    IContextBuilder Clone();
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