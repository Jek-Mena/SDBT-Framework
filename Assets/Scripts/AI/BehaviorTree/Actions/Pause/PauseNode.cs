// TEMPORARY: This node applies a movement override to simulate a pause.
// In the future, replace this with status-based effect handling (e.g., STUN, ROOT).
public class PauseNode : TimedExecutionNode
{
    private bool _modifierApplied;
    private ModifierMeta _meta;

    public override BtStatus Tick(BtController controller)
    {
        var blackboard = controller.Blackboard;

        if (!_modifierApplied && blackboard.MovementLogic != null)
        {
            _meta = ModifierMeta.CreateNow(
                appliedBy: TimedExecutionKeys.Alias.Pause,
                category: ModifierCategories.Movement,
                label: EffectTags.Pause,
                blendMode: ModifierBlendMode.Replace,
                maxStacks: 1,
                duration: blackboard.TimerData.Duration,
                priority: ModifierPriority.Pause,
                isExclusive: true,
                description: "PauseNode applied movement override"
            );

            var modifier = new MovementModifier(_meta, new MovementSettings { IsControlled = true });

            blackboard.MovementModifiers.Add(modifier);
            _modifierApplied = true;
        }

        var status = base.Tick(controller); // Handles timer logic

        if (status != BtStatus.Running && _modifierApplied)
        {
            blackboard.MovementModifiers.Remove(_meta.AppliedBy);
            _modifierApplied = false;
        }

        return status;
    }
}