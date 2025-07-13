using System;
using AI.BehaviorTree.Runtime.Context;
using Systems.StatusEffectSystem.Component;
using UnityEngine;
using Utils.Component;

namespace AI.BehaviorTree.Registry.ContextBuilderModules
{
    public class StatusEffectManagerBuilderModule : IContextBuilderModule
    {
        private const string ScriptName = nameof(StatusEffectManagerBuilderModule);

        public void Build(BtContext context)
        {
            var agent = context.Agent;
            var blackboard = context.Blackboard;
            
            var effect = agent.RequireComponent<StatusEffectManager>();
      
            blackboard.StatusEffectManager = effect;
            
            Debug.Log($"[{ScriptName}] Injected {nameof(StatusEffectManager)} for {agent.name}");
        }
    }
}