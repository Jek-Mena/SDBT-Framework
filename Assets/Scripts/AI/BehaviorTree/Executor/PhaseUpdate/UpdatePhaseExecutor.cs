using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviorTree.Executor.PhaseUpdate
{
    public class UpdatePhaseExecutor : MonoBehaviour
    {
        private readonly Queue<IUpdatePhaseAction> _updateQueue = new();
        private readonly Queue<IUpdatePhaseAction> _lateUpdateQueue = new();
        private readonly Queue<IUpdatePhaseAction> _fixedUpdateQueue = new();

        public void Enqueue(IUpdatePhaseAction action)
        {
            switch (action)
            {
                case IUpdateAction update: 
                    _updateQueue.Enqueue(update); 
                    break;
                case ILateUpdateAction late: 
                    _lateUpdateQueue.Enqueue(late); 
                    break;
                case IFixedUpdateAction fixedAction: 
                    _fixedUpdateQueue.Enqueue(fixedAction); 
                    break;
                default:
                    Debug.LogError("Unknown phase for action.");
                    break;
            }
        }

        private void Update()
        {
            while (_updateQueue.Count > 0)
            {
                var action = _updateQueue.Dequeue();
                action.Execute();
            }
        }

        private void LateUpdate()
        {
            while (_lateUpdateQueue.Count > 0)
            {
                var action = _lateUpdateQueue.Dequeue();
                action.Execute();
            }
        }

        private void FixedUpdate()
        {
            while (_fixedUpdateQueue.Count > 0)
            {
                var action = _fixedUpdateQueue.Dequeue();
                action.Execute();
            }
        }
    }
}