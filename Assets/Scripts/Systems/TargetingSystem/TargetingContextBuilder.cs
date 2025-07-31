using System.Collections.Generic;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using Keys;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Systems.TargetingSystem
{
    public class TargetingContextBuilder : IContextBuilderModule
    {
        public void Build(BtContext context)
        {
            var config = context.AgentDefinition.Config;

            // Get the priority keys
            JArray targetingPriorityArray = null;
            if (config[BtAgentJsonFields.AIConfigField] is JObject aiConfig)
                targetingPriorityArray = aiConfig[BtAgentJsonFields.AIConfig.TargetingPriorityList] as JArray;

            var priorityKeys = targetingPriorityArray != null
                ? targetingPriorityArray.ToObject<List<string>>()
                : new List<string> { BtAgentJsonFields.BehaviorProfiles.DefaultTarget }; // Fallback

            // Get ALL targeting profiles
            var targetingProfiles = context.AgentProfiles.TargetingProfiles;

            var priorities = new List<TargetingData>();
            foreach (var key in priorityKeys)
            {
                if (targetingProfiles != null && targetingProfiles.TryGetValue(key, out var data))
                    priorities.Add(data);
                else
                    Debug.LogWarning($"[TargetingContextBuilder] Missing targeting profile: {key}");
            }

            var coordinator = new TargetCoordinator(context.Agent, priorities);
            context.Blackboard.TargetCoordinator = coordinator;
            context.Controller.RegisterUpdatable(coordinator);
        }

    }
}