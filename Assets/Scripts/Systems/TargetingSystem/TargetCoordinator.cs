using System.Collections.Generic;
using AI.BehaviorTree.Runtime.Context;
using Systems.Abstractions;
using UnityEngine;

namespace Systems.TargetingSystem
{
    public class TargetCoordinator : ISystemUpdatable
    {
        private readonly List<TargetingData> _targetingPriorityList;
        private readonly GameObject _self;

        public TargetCoordinator(GameObject self, List<TargetingData> targetingPriorityList)
        {
            _self = self;
            _targetingPriorityList = targetingPriorityList;
        }
        
        public void Update(BtContext context)
        {
            ref var data = ref context.Blackboard.DataRef;
            
            // Check formation slot (already in struct)
            if (data.FormationPosition.x != 0 || data.FormationPosition.z != 0)
            {
                var agentPos = context.Agent.transform.position;
                var formationPos = new Vector3(
                    data.FormationPosition.x,
                    data.FormationPosition.y,
                    data.FormationPosition.z
                );
            
                var dist = Vector3.Distance(agentPos, formationPos);
                if (dist > 0.5f) // TODO: Make data-driven
                {
                    // Keep formation as priority
                    data.CurrentTargetId = 0; // 0 means use position
                    return;
                }
            }
            
            // Find target based on priority list
            GameObject resolvedTarget = null;
        
            foreach (var targetingData in _targetingPriorityList)
            {
                if (!TargetResolverRegistry.TryGetValue(targetingData.Style, out var resolver))
                    continue;

                var candidate = resolver.ResolveTarget(_self, targetingData, context);
                if (candidate != null || targetingData.AllowNull)
                {
                    resolvedTarget = candidate ? candidate.gameObject : null;
                    break;
                }
            }
        
            // Update struct directly
            if (resolvedTarget != null)
            {
                data.CurrentTargetId = Blackboard.RegisterEntity(resolvedTarget);
                data.FormationPosition = default; // Clear formation
            }
            else
            {
                data.CurrentTargetId = 0;
            }
        }
    }
}