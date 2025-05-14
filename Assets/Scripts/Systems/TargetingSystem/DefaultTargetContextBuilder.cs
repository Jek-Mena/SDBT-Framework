using UnityEngine;

/// <summary>
/// Injects a default target into the blackboard using a specified Unity tag.
/// 
/// Purpose:
/// - This is a plug-and-play IContextBuilderModule for entities that need a known target (e.g., Player).
/// - Commonly used for enemy units or turrets that always lock onto a fixed entity.
/// 
/// Behavior:
/// - If a target is already assigned, it skips (non-destructive).
/// - Searches for a GameObject by tag (defaults to "Player").
////
/// Design Notes:
/// - Part of the IContextBuilderModule system, not a full context builder.
/// - Used *within* a ModularContextBuilder (IContextBuilder) during BtLoader.ApplyAll().
/// - Only modifies the `Target` field of the blackboard. Nothing else.
/// 
/// ⚠️ This is not a targeting system. This is a dumb initializer.
/// 
/// Example Usage:
///   builder.RegisterModule(new DefaultTargetContextBuilder("Player"));
/// 
/// For more complex logic (e.g., closest enemy, first in path), use a TargetResolver or
/// replace this with a smarter module later.
/// 
/// TODO: “Replace DefaultTargetContextBuilder with dynamic target resolver if gameplay expands.”
/// </summary>
public class DefaultTargetContextBuilder : IContextBuilderModule
{
    private readonly string _targetTag;

    /// <summary>
    /// Constructs the module with a specific Unity tag to search for.
    /// </summary>
    /// <param name="targetTag">The Unity tag of the target entity. Defaults to "Player".</param>
    public DefaultTargetContextBuilder(string targetTag = "Player")
    {
        _targetTag = targetTag;
    }

    /// <summary>
    /// Injects a target Transform into the blackboard if none is already assigned.
    /// </summary>
    /// <param name="entity">The entity being initialized.</param>
    /// <param name="blackboard">The shared runtime blackboard to populate.</param>
    public void Build(GameObject entity, Blackboard blackboard)
    {
        if (blackboard.Target != null)
        {
            Debug.Log($"[DefaultTargetContextBuilder] Target already set for {entity.name}, skipping.");
            return;
        }

        var target = GameObject.FindGameObjectWithTag(_targetTag);

        if (target != null)
        {
            blackboard.Target = target.transform;
            Debug.Log($"[DefaultTargetContextBuilder] Target assigned to {target.name} with 'Tag: {_targetTag}'");
        }
        else
        {
            Debug.LogWarning($"[DefaultTargetContextBuilder] No entity with tag '{_targetTag}'");
        }
    }
}

/*
Folder Structure Suggestion
TargetingSystem/
   │
   ├── Context/                  ← Context builders only (context injection)
   │   └── DefaultTargetContextBuilder.cs
   │
   ├── Resolver/                ← Logic-only resolvers (if/when added)
   │   └── ClosestTargetResolver.cs
   │
   ├── TargetingComponent.cs    ← Optional MonoBehaviour for live targeting
   ├── TargetingKeys.cs         ← Shared constants (e.g. tag names)
   
*/