using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Nodes.Actions.Movement.Data;
using UnityEngine;
using UnityEngine.AI;

namespace AI.BehaviorTree.Nodes.Actions.Movement
{
    public class NavMeshMoveToTargetExecutor : IMovementExecutor
    {
        private const string ScriptName = nameof(NavMeshMoveToTargetExecutor);
        public MoveToTargetNodeType Type => MoveToTargetNodeType.NavMesh;
        
        private readonly NavMeshAgent _agent;
        private MovementData _currentSettings;
        private Vector3 _lastSetDestination;

        private const float DefaultUpdateThreshold = 1.0f;
        
        public NavMeshMoveToTargetExecutor(NavMeshAgent agent)
        {
            if (agent)
                _agent = agent;
            else
                throw new System.ArgumentNullException(nameof(agent));
        }
        
        public bool AcceptMoveIntent(Vector3 destination, MovementData data)
        {
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
            _agent.isStopped = false;
            Debug.Log($"{ScriptName}🤖▶️{_agent.gameObject.name} Starting the NavMeshAgent.");
        }

        public void PauseMovement()
        {
            if (!IsAgentValid()) return;
            _agent.isStopped = true;
            Debug.Log($"{ScriptName}🤖⏸️{_agent.gameObject.name} Pausing the NavMeshAgent.");
        }
        
        public void CancelMovement()
        {
            if (!IsAgentValid()) return;
            _agent.isStopped = true;
            _agent.ResetPath();
            Debug.Log($"{ScriptName}🤖⛔{_agent.gameObject.name} Cancelling the NavMeshAgent's path.");
        }

        public bool IsAtDestination()
        {
            if (!_agent) return false;  // Or throw/log
            // Unity NavMeshAgent logic:
            return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance
                                       && (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
        }

        public bool IsCurrentMove(Vector3 destination, MovementData data)
        {
            // 1. Check if destination is close enough to the last intent (STICKY!)
            var stickyThreshold = data?.UpdateThreshold > 0 ? data.UpdateThreshold : DefaultUpdateThreshold;
            var destinationMatch = Vector3.Distance(_lastSetDestination, destination) < stickyThreshold;

            // 2. Only match mode/type, not deep settings (unless you really care about speed/etc per move)
            var movementTypeMatch = _currentSettings?.MovementType == data?.MovementType;

            // 3. Agent must still be valid
            var agentValid = IsAgentValid();

            return destinationMatch && movementTypeMatch && agentValid;
        }

        private bool IsAgentValid()
        {
            if (!_agent)
            {
                Debug.LogError($"[{ScriptName}]🤖👻 NavMeshAgent is null");
                return false;
            } 
        
            if (!_agent.isOnNavMesh)
            {
                Debug.LogError($"[{ScriptName}]🤖⛔🗺️ NavMeshAgent is NOT on NavMesh");
                return false;
            }

            if (!_agent.enabled)
            {
                Debug.LogError($"[{ScriptName}]🤖🚫 NavMeshAgent is DISABLED");
                return false;
            }
        
            Debug.Log($"{ScriptName}🤖{_agent.name} is valid");
            return true;
        }
        
        // NavMesh doesn't need Tick because NavMeshAgent have its own Tick system
    }
}