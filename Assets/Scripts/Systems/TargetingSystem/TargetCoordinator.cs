using System.Collections.Generic;
using AI.BehaviorTree.Keys;
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
            Vector3? formationSlot = null;
            if (context.Blackboard.TryGet(BlackboardKeys.Target.Formation, out Vector3 slotPos))
                formationSlot = slotPos;
            
            if (formationSlot.HasValue)
            {
                var dist = Vector3.Distance(context.Agent.transform.position, formationSlot.Value);
                var slotTolerance = 0.5f; // tweak as needed

                if (dist > slotTolerance)
                {
                    // Enforce slot discipline
                    context.Blackboard.Set(BlackboardKeys.Target.CurrentTarget, formationSlot.Value);
                    context.Blackboard.Set(BlackboardKeys.Target.CurrentTargetSource, "SlotDiscipline");
                    return; // do not process other priorities!
                }
            }
            
            object resolvedTarget = null;
            string sourceProfile = null;
            
            foreach (var data in _targetingPriorityList)
            {
                if (!TargetResolverRegistry.TryGetValue(data.Style, out var resolver))
                    continue;

                var candidate = resolver.ResolveTarget(_self, data, context);

                if (!candidate && !data.AllowNull) continue;

                resolvedTarget = candidate;
                sourceProfile = data.BlackboardKey;
                break; // Stop at first valid
            }
            
            // This is the **intent arbitration**: set a single unified key
            if (resolvedTarget != null)
            {
                context.Blackboard.Set(BlackboardKeys.Target.CurrentTarget, resolvedTarget);
                context.Blackboard.Set(BlackboardKeys.Target.CurrentTargetSource, sourceProfile); // (Optional: for debug/UI)
            }
            else
            {
                context.Blackboard.Remove(BlackboardKeys.Target.CurrentTarget);
                context.Blackboard.Remove(BlackboardKeys.Target.CurrentTargetSource);
            }
        }
    }
}