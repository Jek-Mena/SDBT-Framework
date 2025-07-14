using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace Systems.StatusEffectSystem.Component
{
    public class StatSynchronizer : MonoBehaviour
    {
        private StatusEffectManager _statusEffectManager;
        private BtContext _context;
    
        public void Initialize(BtContext context) => _context = context;

        public void SetStatusEffectManager(StatusEffectManager manager)
        {
            // Unsubscribe from previous manager (if any) to avoid leaks
            if (_statusEffectManager != null)
                _statusEffectManager.OnStatusEffectChanged -= SyncToBlackboard;

            _statusEffectManager = manager;

            if (_statusEffectManager != null)
                _statusEffectManager.OnStatusEffectChanged += SyncToBlackboard;
        }

        private void SyncToBlackboard()
        {
            var modifiers = _statusEffectManager.agentModifiers.Stats;

            // -- Sync each multipliers to the blackboard
            Debug.Log("Syncing multipliers to blackboard...");
            _context.Blackboard.Set(BlackboardKeys.Multipliers.Movement, modifiers.Movement);
            _context.Blackboard.Set(BlackboardKeys.Multipliers.Attack, modifiers.Attack);
            _context.Blackboard.Set(BlackboardKeys.Multipliers.Armor, modifiers.Armor);
            // etc.
        }

        private void OnDestroy()
        {
            if (_statusEffectManager != null)
                _statusEffectManager.OnStatusEffectChanged -= SyncToBlackboard;
        }
    }
}