using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviorTree.Core
{
    public static class AgentRuntimeRegistry
    {
        private static readonly HashSet<IAgentRuntimeData> Agents = new();
        
        public static void Register(IAgentRuntimeData agentRuntimeData)
        {
            if (agentRuntimeData == null) throw new System.ArgumentNullException(nameof(agentRuntimeData));
            Agents.Add(agentRuntimeData);
        }

        public static void Unregister(IAgentRuntimeData agentRuntimeData)
        {
            if (agentRuntimeData == null) return;
            Agents.Remove(agentRuntimeData);
        }

        public static IEnumerable<IAgentRuntimeData> GetAgentsInRadius(Vector3 agentPosition, float radius)
        {
            var radiusSqr = radius * radius;
            foreach (var agent in Agents)
            {
                if ((agent.Transform.position - agentPosition).sqrMagnitude <= radiusSqr)
                    yield return agent;
            }
        }
    }

    public interface IAgentRuntimeData
    {
        Transform Transform { get; }
    }
}