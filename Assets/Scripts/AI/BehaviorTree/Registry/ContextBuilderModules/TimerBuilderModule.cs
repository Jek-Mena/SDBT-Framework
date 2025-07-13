using System;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;
using Utils.Component;

namespace AI.BehaviorTree.Registry.ContextBuilderModules
{
    public class TimerBuilderModule : IContextBuilderModule
    {
        public void Build(BtContext context)
        {
            var scriptName = nameof(TimerBuilderModule);
            var agent = context.Agent;
            var blackboard = context.Blackboard;
            
            var timer = agent.RequireComponent<TimeExecutionManager>();
        
            if (!timer)
                throw new Exception($"[{scriptName}] {nameof(TimeExecutionManager)} missing on {agent.name}");
        
            blackboard.TimeExecutionManager = timer;
            Debug.Log($"[{scriptName}] Injected {nameof(TimeExecutionManager)} for {agent.name}");
        }
    }
}