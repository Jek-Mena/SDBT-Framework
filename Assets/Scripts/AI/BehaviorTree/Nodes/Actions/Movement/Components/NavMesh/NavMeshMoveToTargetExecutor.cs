using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.Actions.Movement.Data;
using UnityEngine;
using UnityEngine.AI;

namespace AI.BehaviorTree.Nodes.Actions.Movement.Components.NavMesh
{
    public class NavMeshMoveToTargetExecutor : IMovementExecutor
    {
        private const string ScriptName = nameof(NavMeshMoveToTargetExecutor);
        public MovementNodeType Type => MovementNodeType.NavMesh;
        
        private readonly NavMeshAgent _agent;
        private MovementData _currentSettings;
        private Vector3 _lastSetDestination;
        private bool _isMoving;

        public NavMeshMoveToTargetExecutor(NavMeshAgent agent)
        {
            if (agent)
                _agent = agent;
            else
                throw new System.ArgumentNullException(nameof(agent));
        }
        
        public bool TryMoveTo(Vector3 destination)
        {
            Debug.Log($"[NavMeshExecutor] TryMoveTo: called, target={destination}, isMoving={_isMoving}");

            // Check if the agent is valid
            if (!IsAgentValid())
                return false;
            
            // If first move or moved far enough, issue move command
            if (Vector3.Distance(_lastSetDestination, destination) > _currentSettings.UpdateThreshold)
            {
                _lastSetDestination = destination;
                var pathSet = _agent.SetDestination(destination);

                if (pathSet)
                    return true;

                Debug.LogWarning($"[{ScriptName}] {_agent.gameObject.name} failed to SetDestination (may be unreachable or off mesh).");
                return false;
            }
            
            // No move was issued, but technically (hopefully) the agent is still on its way to the last set destination
            return true;
        }

        public void ApplySettings(MovementData data)
        {
            if (_currentSettings != null && _currentSettings == data) return;
            _currentSettings = data;
            
            _agent.speed = data.Speed;
            _agent.angularSpeed = data.AngularSpeed;
            _agent.acceleration = data.Acceleration;
            _agent.stoppingDistance = data.StoppingDistance;
            Debug.Log($"[{ScriptName}] Settings applied to {_agent.gameObject.name}");
        }

        public void StartMovement()
        {
            if (!IsAgentValid()) return;
            _isMoving = true;
            _agent.isStopped = false;
            Debug.Log($"{_agent.gameObject.name} Starting the NavMeshAgent.");
        }

        public void PauseMovement()
        {
            if (!IsAgentValid()) return;
            _isMoving = false;
            _agent.isStopped = true;
            Debug.Log($"{_agent.gameObject.name} Pausing the NavMeshAgent.");
        }
        
        public void CancelMovement()
        {
            if (!IsAgentValid()) return;
            _isMoving = false;
            _agent.isStopped = true;
            _agent.ResetPath();
            Debug.Log($"{_agent.gameObject.name} Cancelling the NavMeshAgent's path.");
        }

        public bool IsAtDestination()
        {
            throw new System.NotImplementedException();
        }
        
        private bool IsAgentValid()
        {
            if (!_agent)
            {
                Debug.LogError($"[{ScriptName}] NavMeshAgent is null");
                return false;
            } 
        
            if (!_agent.isOnNavMesh)
            {
                Debug.LogWarning($"[{ScriptName}] NavMeshAgent is NOT on NavMesh");
                return false;
            }

            if (!_agent.enabled)
            {
                Debug.LogWarning($"[{ScriptName}] NavMeshAgent is DISABLED");
                return false;
            }

            // If agent is stopped, warn and refuse to move
            if (_agent.isStopped)
            {
                Debug.LogWarning($"[{ScriptName}] TryMoveTo called while NavMeshAgent is stopped.");
                return false;
            }
        
            return true;
        }
        
        // NavMesh doesn't need Tick because NavMeshAgent have its own Tick system
    }
}