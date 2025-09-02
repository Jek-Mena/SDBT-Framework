using AI.BehaviorTree.Runtime.Context;
using Unity.VisualScripting;
using UnityEngine;

namespace AI.SquadAI
{
    public class SquadAgent : ISquadAgent
    {
        private string _role;
        private Formation _formation;
        private SquadManager _manager;
        public Transform Transform { get; }

        public SquadAgent(BtContext context, GroupBehaviorProfileEntry groupEntry)
        {
            Transform = context.Agent.transform;
            _role = groupEntry.Role;
        }

        public void SetSquadSlot(Formation slot, SquadManager manager)
        {
            _formation = slot;
            _manager = manager;
        }
        
        public void UpdateFormation()
        {
            return;
            if (_formation == null || _manager == null) return;
            
            Vector3 target;
            if (_formation.AgentIndex == 0)
                target = _manager.GoalPosition; // Leader goes to squad goal
            else
                target = _manager.Leader.Transform.position + 
                         _manager.Leader.Transform.rotation * _formation.Offset;
                
            // Move to slot (use your BT/MovementIntent as normal)
            // For demo, just move directly:
            var speed = 3f;
            var dir = (target - Transform.position);
            if (dir.magnitude > 0.1f)
                Transform.position += dir.normalized * (speed * Time.deltaTime);
            // Leader slot is always slot.Index == 0
        }
        
        public Vector3 GetSlotWorldPosition()
        {
            if (_formation == null || _manager == null) return Transform.position;
            if (_formation.AgentIndex == 0)
                return _manager.GoalPosition;
            else
                return _manager.Leader.Transform.position + _manager.Leader.Transform.rotation * _formation.Offset;
        }

        public bool IsLeader { get; set; }
    }
}