using AI.BehaviorTree.Nodes.Actions.Movement.Components;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.BehaviorTree.Registry.ContextBuilderModules
{
    public class MovementOrchestratorBuilderModule : IContextBuilderModule
    {
        private const string ScriptName = nameof(MovementOrchestratorBuilderModule);
        public void Build(BtContext context)
        {
            var agent = context.Agent;
            var orchestrator = context.MovementOrchestrator;
                
            orchestrator.Initialize(context);
            Debug.Log($"[{ScriptName}] {orchestrator} initialized for {agent.name}");
        }
    }
}