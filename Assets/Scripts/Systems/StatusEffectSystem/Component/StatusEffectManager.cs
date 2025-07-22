using System;
using System.Collections.Generic;
using System.Linq;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace Systems.StatusEffectSystem.Component
{
    /// <summary>
    /// Manages the application, removal, and tracking of status effects within the game.
    /// </summary>
    /// <remarks>
    /// This class is responsible for maintaining a list of active status effects and ensuring their proper application and removal.
    /// It also provides mechanisms to notify other systems when domains are blocked or unblocked due to status effects.
    /// </remarks>
    public class StatusEffectManager : IBlocker, ISystemCleanable
    {
        // TODO: [2025-07-17]
        // Rework DomainBlocked/DomainUnblocked event system.
        // Use a lifecycle-safe, robust event mechanism instead of ?.Invoke.
        // Detect and log/assert when invoking with no listeners to avoid silent state desync.
        // Ensure all listeners are unsubscribed properly during system cleanup to prevent memory leaks or null calls.

        private const string ScriptName = nameof(StatusEffectManager);
        public event Action<string> DomainBlocked; // Called when a new domain block is applied
        public event Action<string> DomainUnblocked; // Called when a domain in unblocked (effect removed)
        public event Action OnStatusEffectChanged;
        
        private readonly List<StatusEffect> _activeEffects = new(); // Tracks all currently active status effects
        private readonly Dictionary<string, HashSet<string>> _domainToEffectIds = new(); // Track all blocked domains and which effects block them
        
        public AgentModifiers AgentModifiers { get; private set; } = new();
        public IEnumerable<StatusEffect> GetActiveEffects() => _activeEffects;
        
        // Applies a new status effect to the manager.
        // This adds the effect to the active list and triggers any relevant domain block events.
        public void ApplyEffect(StatusEffect effect)
        {
            //Debug.Log($"[StatusEffectManager] Applying effect: {effect.Name}, Domains: {string.Join(",", effect.Domains)}");
            
            if(!_activeEffects.Contains(effect))
                _activeEffects.Add(effect);
            
            foreach (var domain in effect.Domains)
            {
                if(!_domainToEffectIds.TryGetValue(domain, out var effectIds))
                    _domainToEffectIds[domain] = effectIds = new HashSet<string>();

                var wasBlocked = effectIds.Count > 0;
                effectIds.Add(effect.Id);

                if (wasBlocked) continue;
                DomainBlocked?.Invoke(domain); // use ?. for safety!
                //Debug.Log($"[{ScriptName}] First block for domain: {domain}, invoking DomainBlocked");
            }
            RecalculateModifiers();
            // TODO: Handle stacking, priority, expiry, etc. (reuse ModifierMeta/Stack pattern)
        }
        
        public void RemoveEffects(StatusEffect effect)
        {
            if (!_activeEffects.Remove(effect)) return;
            
            foreach (var domain in effect.Domains)
            {
                if (!_domainToEffectIds.TryGetValue(domain, out var effectIds)) continue;
                effectIds.Remove(effect.Id);
                
                if (effectIds.Count > 0) continue;
                _domainToEffectIds.Remove(domain);
                DomainUnblocked?.Invoke(domain);
                Debug.Log($"[{ScriptName}] Last unblock for domain: {domain}, invoking DomainUnblocked");
            }
            RecalculateModifiers();
        }

        public bool IsBlocked(string domain)
        {
            return _domainToEffectIds.TryGetValue(domain, out var effectIds) && effectIds.Count > 0;
        }
        
        private void RecalculateModifiers()
        {
            AgentModifiers.Stats.Reset();
            foreach (var effect in _activeEffects)
                AgentModifiers.Stats.MultiplyWith(effect.Multipliers.Stats);
            OnStatusEffectChanged?.Invoke();
        }

        public void ReleaseSystem(BtContext context)
        {
            _activeEffects.Clear();
            _domainToEffectIds.Clear();
            AgentModifiers.Stats.Reset(); // Not yet properly implemented
            OnStatusEffectChanged?.Invoke();

            //Debug.Log($"[{ScriptName}] After cleanup, activeEffects.Count = {_activeEffects.Count}");
            foreach (var effect in _activeEffects)
                Debug.Log($"[{ScriptName}] Leaked effect: {effect}, Domains: {string.Join(",", effect.Domains)}"); 
        }
        
        // Removes inactive status effects at the end of the frame
        public void LateTick()
        {
            var expired = _activeEffects.Where(e => !e.IsActive()).ToList();
            foreach (var effect in expired)
                RemoveEffects(effect);
        }
    }
}