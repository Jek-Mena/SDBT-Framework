using System;
using UnityEngine;

public class TimerContextBuilder : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(TimerContextBuilder);
        var agent = context.Agent;
        var blackboard = context.Blackboard;
            
        var timer = agent.RequireComponent<TimeExecutionManager>();
        if (!timer)
            throw new Exception($"[{scriptName}] {nameof(TimeExecutionManager)} missing on {agent.name}");
        blackboard.TimeExecutionManager = timer;
        Debug.Log($"[{scriptName}] Injected {nameof(TimeExecutionManager)} for {agent.name}");
    }
}