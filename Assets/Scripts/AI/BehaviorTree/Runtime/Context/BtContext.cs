// [PHASE 1: Introduce for internal use only. Full migration in PHASE 3]

using UnityEngine;

/// <summary>
/// Represents the context for a behavior tree execution in an AI system.
/// This context encapsulates references to the AI entity itself, its blackboard for shared data,
/// and the behavior tree controller managing the execution flow.
/// </summary>
public class BtContext
{
    public GameObject Agent { get; }
    public Blackboard Blackboard { get; }
    public BtController Controller { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BtContext"/> class.
    /// </summary>
    /// <param name="self">
    /// The <see cref="GameObject"/> representing the AI entity associated with this context.
    /// </param>
    /// <param name="blackboard">
    /// The <see cref="Blackboard"/> instance containing shared data for behavior tree nodes.
    /// </param>
    /// <param name="controller">
    /// The <see cref="BtController"/> managing the behavior tree execution flow.
    /// </param>
    public BtContext(BtController controller)
    {
        Controller = controller;
        Agent = controller.gameObject;
        Blackboard = controller.Blackboard;
    }
    
    // Facade Properties for leaf node convenience.

    public TargetingData TargetingData => Blackboard.TargetingData;
    public ITargetResolver TargetResolver =>
        Blackboard.TargetResolver ?? TargetResolverRegistry.TryGetValue(TargetingData.Style);
    public IMovementNode Movement => Blackboard.MovementLogic;
    public IRotationNode Rotation => Blackboard.RotationLogic;
    public TimerExecutionMono Timers => Blackboard.TimeExecutionManager;
    public StatusEffectManager StatusEffectManager => Blackboard.StatusEffectManager;
    public UpdatePhaseExecutor UpdatePhaseExecutor => Blackboard.UpdatePhaseExecutor;

    public bool IsValid =>
        Controller is not null &&
        Blackboard != null &&
        Movement != null &&
        TargetingData != null;
}