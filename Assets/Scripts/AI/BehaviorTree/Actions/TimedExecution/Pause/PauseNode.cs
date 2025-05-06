// TEMPORARY: This node applies a movement override to simulate a pause.
// In the future, replace this with status-based effect handling (e.g., STUN, ROOT).

using UnityEngine;

[BtNode(BtNodeName.Tasks.Pause)]
public class PauseNode : TimedExecutionNode 
{
    private bool _modifierApplied;
    private ModifierMeta _meta;

    public override BtStatus Tick(BtController controller)
    {
        var blackboard = controller.Blackboard;

        // First tick -> stop movement
        if (!_modifierApplied && blackboard.MovementLogic != null)
        {
            _meta = new ModifierMeta (
                source: Modifiers.Pause,
                priority: ModifierPriority.Pause,
                effectTag: EffectTags.Movement,
                blendMode: ModifierBlendMode.Override
            );

            var modifier = new MovementModifier(_meta, new MovementSettings { IsControlled = true });

            blackboard.MovementModifiers.Add(modifier);
            _modifierApplied = true;
        }

        var status = base.Tick(controller); // Handles the actual timer logic

        // On completion, remove modifier
        if (status != BtStatus.Running && _modifierApplied)
        {
            blackboard.MovementModifiers.Remove(_meta.Source);
            _modifierApplied = false;
        }

        return status;
    }
}