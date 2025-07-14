using System;
using System.Linq;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.BehaviorTree.Registry.ContextBuilderModules.Abstraction
{
    public class PerceptionBuilderModule : IContextBuilderModule
    {
        public void Build(BtContext context)
        {
            var scriptName = nameof(PerceptionBuilderModule);
            var agent = context.Agent;
            var blackboard = context.Blackboard;
        
            // Find all attached perception modules (of any generic type)
            var perceptionModules = agent.GetComponents<MonoBehaviour>()
                .Where(c => c is IPerceptionModule)
                .Cast<IPerceptionModule>()
                .ToList();

            if (!perceptionModules.Any())
            {
                Debug.LogError($"[{scriptName}] No PerceptionModules found on {agent.name}");
                throw new Exception($"No PerceptionModules found on {agent.name}");
            }
        
            Debug.Log(
                $"[{scriptName}] Injecting {perceptionModules.Count} PerceptionModules for '{agent.name}':\n" +
                string.Join("- ", perceptionModules.ConvertAll(m => m.GetType().Name).Prepend("")) // shows type names
            );
        
            foreach (var module in perceptionModules)
            {
                try
                {
                    module.Initialize(context);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[{scriptName}] ERROR initializing {module.GetType().Name} " +
                                   $"for '{agent.name}': {ex.Message}\n{ex.StackTrace}" );
                }
            }
            
            context.Blackboard.PerceptionModules = perceptionModules;
        }
    }
}