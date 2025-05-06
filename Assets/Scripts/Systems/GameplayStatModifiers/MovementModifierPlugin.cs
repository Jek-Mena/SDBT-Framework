// TODO: Split into modular sub-context builders by domain:
// IContextBuilder -> MovementContextBuilder, HealthContextBuilder, etc.
// Register them via BtServices and compose dynamically in Build().

using Newtonsoft.Json.Linq;
using UnityEngine;

[Plugin(PluginKey.BtNodeModifier_Movement)]
public class MovementModifierPlugin : BasePlugin
{
    public override void Apply(GameObject entity, JObject config)
    {
        var provider = new MovementSettingsModifierProvider();

        var mover = entity.GetComponent<NavMeshMover>();
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