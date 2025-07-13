using AI.BehaviorTree.Nodes.Actions.Movement.Components;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;
using Utils.Component;

namespace AI.BehaviorTree.Registry.ContextBuilderModules
{
    public class MovementOrchestratorBuilderModule : IContextBuilderModule
    {
        private const string ScriptName = nameof(MovementOrchestratorBuilderModule);
        public void Build(BtContext context)
        {
            var agent = context.Agent;
            var blackboard = context.Blackboard;
            
            var orchestrator = agent.RequireComponent<MovementOrchestrator>();
            
            orchestrator.Initialize(context);
            
            blackboard.MovementOrchestrator = orchestrator;
            
            Debug.Log($"[{ScriptName}] {orchestrator} initialized for {agent.name}");
        }
    }
}