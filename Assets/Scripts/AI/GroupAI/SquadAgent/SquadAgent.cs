using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.GroupAI.SquadAgent
{
    public class SquadAgent : ISquadAgent
    {
        public Formation Formation { get; set; }
        public SquadManager SquadManager { get; set; }
        public Transform Transform { get; }
        
        private readonly BtContext _context;

        public SquadAgent(BtContext context, FormationProfileEntry entry)
        {
            _context = context;
            Transform = context.Agent.transform;
        }

        public void SetSquadSlot(Formation slot, SquadManager manager)
        {
            Formation = slot;
            SquadManager = manager;
        }

        public void UpdateFormation()
        {
            if (Formation == null || SquadManager == null || _context == null) return;

            var slotWorldPos = GetSlotWorldPosition();
            _context.Blackboard.Set(BlackboardKeys.Target.Formation, slotWorldPos);
        }

        public Vector3 GetSlotWorldPosition()
        {
            // Slot 0 (leader): free movement, ignores formation slot
            if (IsLeader)
                return Transform.position;

            // Fallback: If not in formation, just stay in place (or return target if solo)
            if (Formation == null || SquadManager == null)
                return Transform.position;

            var leader = SquadManager.GetLeader();
            if (leader == null)
                return Transform.position;

            // Formation offset is in local (leader) space
            var offset = Formation.Offset;
            return leader.Transform.position + leader.Transform.rotation * offset;
        }

        public bool IsLeader { get; set; }
    }
}