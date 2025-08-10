using System.Collections.Generic;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using Systems.Abstractions;
using Unity.Mathematics;
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
            
            Vector3? formationSlot = null;
            if (context.Blackboard.TryGet(BlackboardKeys.Target.Formation, out Vector3 slotPos))
                formationSlot = slotPos;
            
            if (formationSlot.HasValue)
            {
                var dist = Vector3.Distance(context.Agent.transform.position, formationSlot.Value);
                // TODO Make this data driven, also TargetCoordinator is not yet finished.
                var slotTolerance = 0.5f; // tweak as needed

                if (dist > slotTolerance)
                {
                    // Struct as the source of truth
                    data.CurrentTargetId = 0; // 0 == position target
                    var fp = formationSlot.Value;
                    data.FormationPosition = new float3(fp.x, fp.y, fp.z);
                    return; // do not process other priorities!
                }
            }
            
            object resolvedTarget = null;
            string sourceProfile = null;
            
            foreach (var targetingData in _targetingPriorityList)
            {
                if (!TargetResolverRegistry.TryGetValue(targetingData.Style, out var resolver))
                    continue;

                var candidate = resolver.ResolveTarget(_self, targetingData, context);
                if (!candidate && !targetingData.AllowNull) continue;

                resolvedTarget = candidate;
                sourceProfile = targetingData.BlackboardKey;
                break; // Stop at first valid
            }
            
            // This is the **intent arbitration**: set a single unified key
            if (resolvedTarget != null)
            {
                // Struct as the source of truth
                if (resolvedTarget is Transform tr)
                    data.CurrentTargetId = tr.gameObject.GetInstanceID();
                else if (resolvedTarget is GameObject go)
                    data.CurrentTargetId = go.GetInstanceID();

                data.FormationPosition = default; // clear any stale slot
            }
            else
            {
                data.CurrentTargetId   = 0;
                data.FormationPosition = default;
            }
        }
    }
}