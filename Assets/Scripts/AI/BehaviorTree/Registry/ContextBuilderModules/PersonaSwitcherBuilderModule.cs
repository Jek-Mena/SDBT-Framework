using AI.BehaviorTree.Runtime.Context;
using AI.BehaviorTree.Switching;
using UnityEngine;
using Utils.Component;

namespace AI.BehaviorTree.Registry.ContextBuilderModules
{
    public class PersonaSwitcherBuilderModule : IContextBuilderModule
    {
        private const string ScriptName = nameof(PersonaSwitcherBuilderModule);
        
        public void Build(BtContext context)
        {
            var agent = context.Agent;
            var blackboard = context.Blackboard;

            var personaSwitcher = agent.RequireComponent<BtPersonaSwitcher>();
            
            personaSwitcher.Initialize(context);
            
            blackboard.PersonaSwitcher = personaSwitcher;
            
            Debug.Log($"[{ScriptName}] {personaSwitcher} initialized for {agent.name}");
        }
    }
}