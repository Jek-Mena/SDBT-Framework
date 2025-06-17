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
    
    public BtContext(BtController controller, Blackboard blackboard, GameObject agent)
    {
        Controller = controller;
        Blackboard = blackboard;
        Agent = agent;
    }
    
    // Facade Properties for leaf node convenience.

    public TargetingData TargetingData => Blackboard.TargetingData;
    public ITargetResolver TargetResolver =>
        Blackboard.TargetResolver ?? TargetResolverRegistry.TryGetValue(TargetingData.Style);
    public IMovementNode Movement => Blackboard.MovementLogic;
    public IRotationNode Rotation => Blackboard.RotationLogic;
    public TimeExecutionManager TimeExecutionManager => Blackboard.TimeExecutionManager;
    public StatusEffectManager StatusEffectManager => Blackboard.StatusEffectManager;
    public UpdatePhaseExecutor UpdatePhaseExecutor => Blackboard.UpdatePhaseExecutor;

    public bool IsValid =>
        Controller is not null &&
        Blackboard != null &&
        Movement != null &&
        TargetingData != null;
}