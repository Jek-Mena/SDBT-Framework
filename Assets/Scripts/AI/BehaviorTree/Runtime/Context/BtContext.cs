using UnityEngine;

/// <summary>
/// [2025-06-18] Only construct via the builder pipeline. Never new up outside context build!
/// Represents the context for a behavior tree execution in an AI system.
/// This context encapsulates references to the AI entity itself, its blackboard for shared data,
/// and the behavior tree controller managing the execution flow.
///
/// [ARCHITECTURAL RULE]
/// There is one BtContext per entity. It is constructed only by the context builder pipeline.
/// All systems must use the canonical instance set on BtController.Context.
/// Never new up a context in plugins, nodes, or runtime.
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
    public IMovementNode Movement => Blackboard.MovementLogic;
    public IRotationNode Rotation => Blackboard.RotationLogic;
    public TimeExecutionManager TimeExecutionManager => Blackboard.TimeExecutionManager;
    public StatusEffectManager StatusEffectManager => Blackboard.StatusEffectManager;
    public UpdatePhaseExecutor UpdatePhaseExecutor => Blackboard.UpdatePhaseExecutor;
}