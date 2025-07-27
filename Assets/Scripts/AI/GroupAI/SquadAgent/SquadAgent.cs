using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.GroupAI.SquadAgent
{
    public class SquadAgent : ISquadAgent
    {
        private string _role;
        private Formation _formation;
        private SquadManager _manager;
        public Transform Transform { get; }

        public SquadAgent(BtContext context, FormationProfileEntry entry)
        {
            Transform = context.Agent.transform;
            _role = entry.Role;
        }

        public void SetSquadSlot(Formation slot, SquadManager manager)
        {
            _formation = slot;
            _manager = manager;
        }
        
        public void UpdateFormation()
        {
            if (_formation == null || _manager == null) return;
            
            Vector3 target;
            // Compute direction toward goal
            var toGoal = (_manager.GoalPosition - _manager.Leader.Transform.position);
            var forward = toGoal.sqrMagnitude > 0.01f ? toGoal.normalized : _manager.Leader.Transform.forward;
            var formationRotation = Quaternion.LookRotation(forward, Vector3.up);

            // Compute target position
            if (_formation.AgentIndex == 0)
            {
                target = _manager.GoalPosition; // Leader goes to the goal
            }
            else
            {
                target = _manager.Leader.Transform.position + formationRotation * _formation.Offset;
            }

            // Movement (replace this with your own movement intent system)
            var speed = 3f;
            var dir = target - Transform.position;
            if (dir.magnitude > 0.1f)
            {
                Transform.position += dir.normalized * (speed * Time.deltaTime);
            }
        }
        
        public Vector3 GetSlotWorldPosition()
        {
            if (_formation == null || _manager == null) return Transform.position;

            var toGoal = (_manager.GoalPosition - _manager.Leader.Transform.position);
            var forward = toGoal.sqrMagnitude > 0.01f ? toGoal.normalized : _manager.Leader.Transform.forward;
            var formationRotation = Quaternion.LookRotation(forward, Vector3.up);

            if (_formation.AgentIndex == 0)
                return _manager.GoalPosition;
            else
                return _manager.Leader.Transform.position + formationRotation * _formation.Offset;
        }

        public bool IsLeader { get; set; }
    }
}