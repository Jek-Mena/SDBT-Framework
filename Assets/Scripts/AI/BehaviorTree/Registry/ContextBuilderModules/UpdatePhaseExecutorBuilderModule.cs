using System;
using AI.BehaviorTree.Executor.PhaseUpdate;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.BehaviorTree.Registry.ContextBuilderModules
{
    public class UpdatePhaseExecutorBuilderModule : IContextBuilderModule
    {
        public void Build(BtContext context)
        {
            var scriptName = nameof(UpdatePhaseExecutorBuilderModule);
            var agent = context.Agent;
            var updatePhaseExecutor = context.Services.UpdatePhase;
            
            var exec = agent.GetComponent<UpdatePhaseExecutor>();
            if (!exec)
                throw new Exception($"[{scriptName}] {nameof(UpdatePhaseExecutor)} missing on {agent.name}");
            updatePhaseExecutor = exec;
            Debug.Log($"[{scriptName}] Injected {nameof(UpdatePhaseExecutor)} for {agent.name}");
        }
    }
}