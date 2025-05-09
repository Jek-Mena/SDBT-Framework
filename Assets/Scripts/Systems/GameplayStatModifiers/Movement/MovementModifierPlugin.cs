// TODO: Split into modular sub-context builders by domain:
// IContextBuilder -> MovementContextBuilder, HealthContextBuilder, etc.
// Register them via BtServices and compose dynamically in Build().

using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtNodeModifier_Movement)]
public class MovementModifierPlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject jObject)
    {
        var provider = new MovementSettingsModifierProvider();

        var mover = entity.GetComponent<NavMeshMoveToTarget>();
        if (mover != null)
            mover.SetSettingsProvider(provider);

        var controller = entity.GetComponent<BtController>();
        if (controller == null || controller.Blackboard == null)
        {
            Debug.LogWarning($"[MovementModifierPlugin] Missing BtController or Blackboard on {entity.name}. Attempting to build context...");
            BtServices.ContextBuilder?.Build(entity);

            controller = entity.GetComponent<BtController>();
            if (controller == null || controller.Blackboard == null)
            {
                Debug.LogError($"[MovementModifierPlugin] ContextBuilder failed to resolve controller or blackboard on {entity.name}.");
                return;
            }
        }

        controller.Blackboard.MovementModifiers = provider;
        Debug.Log($"[MovementModifierPlugin] Applied ModifierProvider to {entity.name}");
    }
}

// ================================
// TODO: Integrate Runtime Execution into Modifier Framework
// ================================
// CONTEXT:
// MovementModifier currently leverages the shared IModifier<T> interface from GameplayStatModifiers/Core.
// Static modifiers are applied at setup via MovementSettingsModifierProvider.
// The base system supports stacking, priorities, and blend modes,
// but NOT runtime behavior like expiration, timed execution, or repeat logic.
//
// PLAN:
// [ ] Add runtime lifecycle execution to modifiers using ModifierRuntimeExecutor.
// [ ] Possibly place in `GameplayStatModifiers/Runtime/`
//     - Will manage timed application, repeat intervals, and expirations.
// [ ] Extend ModifierMeta (or embed into MovementModifier) with optional runtime metadata:
//     - float? Duration
//     - int RepeatCount
//     - float RepeatInterval
// [ ] Executor will use ModifierStack<T> but add time-awareness (e.g., tracked by coroutine or tick).
// [ ] Allow plugin or BT nodes to schedule modifiers with runtime semantics.
//
// LONG-TERM:
// This unifies the design: stateless modifiers remain lightweight, while
// dynamic effects gain runtime lifecycle support without polluting behavior trees
// or over-complicating modifier application logic.

// ================================
// TODO: Consolidate Plugin Execution with a Central PluginManager
// ================================
// CONTEXT:
// Plugins are currently scattered and manually applied (e.g., MovementModifierPlugin).
// There is no centralized PluginManager or clear ownership over plugin discovery, registration, or execution.
// The [Plugin(...)] attribute is likely used to tag plugin types, but no unified handler or factory manager is in place.
//
// PROBLEM:
// - No single source of truth for plugin registration and execution
// - Discovery is likely manual or fragmented
// - Hard to know what plugins are available or used at runtime
//
// GOAL:
// Introduce a `PluginManager` as the central runtime coordinator for plugin lifecycle:
//   - Discover plugins using reflection or static registration
//   - Register by key (e.g., PluginKey.BtNodeModifier_Movement)
//   - Expose Apply(entity, PluginKey, JObject) interface
//
// PLAN:
// [ ] Search entire project for `[Plugin(...)]` attributes or classes implementing IPlugin/BasePlugin
// [ ] Identify where (if anywhere) plugins are currently registered or called (likely in BtServices or BtContextBuilder)
// [ ] Create `PluginManager` static class or DI service:
//       - Dictionary<PluginKey, IPlugin>
//       - Register(), Get(), Apply()
// [ ] Move all plugin registration to this manager (via reflection-based discovery or manual setup)
// [ ] Optional: Add logging or validation on unknown keys or duplicate plugin registration
//
// OUTCOME:
// All plugin logic becomes traceable and injectable from one place.
// Allows better debugging, runtime patching, and future dynamic loading (e.g., hot-reload support).
