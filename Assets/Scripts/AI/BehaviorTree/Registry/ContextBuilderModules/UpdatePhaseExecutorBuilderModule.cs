using System;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

public class UpdatePhaseExecutorBuilderModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(UpdatePhaseExecutorBuilderModule);
        var agent = context.Agent;
        var blackboard = context.Blackboard;
            
        var exec = agent.GetComponent<UpdatePhaseExecutor>();
        if (!exec)
            throw new Exception($"[{scriptName}] {nameof(UpdatePhaseExecutor)} missing on {agent.name}");
        blackboard.UpdatePhaseExecutor = exec;
        Debug.Log($"[{scriptName}] Injected {nameof(UpdatePhaseExecutor)} for {agent.name}");
    }
}