using System;
using UnityEngine;

public class UpdatePhaseExecutorContextBuilder : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(UpdatePhaseExecutorContextBuilder);
        var agent = context.Agent;
        var blackboard = context.Blackboard;
            
        var exec = agent.GetComponent<UpdatePhaseExecutor>();
        if (!exec)
            throw new Exception($"[{scriptName}] {nameof(UpdatePhaseExecutor)} missing on {agent.name}");
        blackboard.UpdatePhaseExecutor = exec;
        Debug.Log($"[{scriptName}] Injected {nameof(UpdatePhaseExecutor)} for {agent.name}");
    }
}