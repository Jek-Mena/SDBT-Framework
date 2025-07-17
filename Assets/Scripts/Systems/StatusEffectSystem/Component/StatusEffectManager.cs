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
        // Called when a new domain block is applied
        public event Action<string> DomainBlocked;
        // Called when a domain in unblocked (effect removed)
        public event Action<string> DomainUnblocked;
        public event Action OnStatusEffectChanged;
    
        // Tracks all currently active status effects
        private List<StatusEffect> _activeEffects = new();

        public AgentModifiers agentModifiers { get; private set; } = new();
    
        // Retrieves all currently active status effects
        public IEnumerable<StatusEffect> GetActiveEffects() => _activeEffects;
    
        // Removes inactive status effects at the end of the frame
        public void LateTick()
        {
            var expired = _activeEffects.Where(e => !e.IsActive()).ToList();

            foreach (var effect in expired)
            {
                RemoveEffects(effect);
            }
        }

        // Applies a new status effect to the manager.
        // This adds the effect to the active list and triggers any relevant domain block events.
        public void ApplyEffect(StatusEffect effect)
        {
            Debug.Log($"[StatusEffectManager] Applying effect: {effect}, Domains: {effect.Domains}, DomainBlocked: {DomainBlocked}");
            _activeEffects.Add(effect);
            foreach (var domain in effect.Domains)
            {
                Debug.Log($"[StatusEffectManager] Processing domain: {domain}");
                if(IsFirstBlock(domain))
                {
                    Debug.Log($"[StatusEffectManager] First block for domain: {domain}, invoking DomainBlocked");
                    DomainBlocked?.Invoke(domain); // use ?. for safety!
                }
            }
            // TODO: Handle stacking, priority, expiry, etc. (reuse ModifierMeta/Stack pattern)
        }
    
        // Removes a specific status effect from the manager
        public void RemoveEffects(StatusEffect effect)
        {
            _activeEffects.Remove(effect);
            foreach (var domain in effect.Domains)
            {
                if (!IsBlocked(domain))
                    DomainUnblocked?.Invoke(domain);
            }

        }
        
        // Checks if this is the first time the given domain is being blocked.
        // Returns true if no other active effects are already blocking the domain; otherwise, returns false.
        private bool IsFirstBlock(string domain)
        {
            var count = 0;
            foreach (var effect in _activeEffects)
            {
                if (effect.IsActive() && effect.AffectsDomain(domain))
                    count++;
            
            }
            // 0 before add, 1 after (this is first)
            return count == 1;
        }
    
        // Checks if the specified domain is currently blocked by any active status effect.
        // Iterates through all active effects to determine if any affect the given domain
        // and are still active. Returns true if a match is found; otherwise, returns false.
        public bool IsBlocked(string domain)
        {
            foreach (var effect in _activeEffects)
            {
                if (effect.AffectsDomain(domain) && effect.IsActive())
                    return true;
            }

            return false;
        }

        private void RecalculateModifiers()
        {
            // Reset all
            agentModifiers.Stats.Reset();
            // Loop over all effects and modify as needed
            foreach (var effect in _activeEffects)
                agentModifiers.Stats.MultiplyWith(effect.Multipliers.Stats);
            OnStatusEffectChanged?.Invoke();
        }

        public void ReleaseSystem(BtContext context)
        {
            // Remove all effects immediately
            _activeEffects.Clear();

            // Reset all modifiers
            agentModifiers.Stats.Reset(); // Not yet properly implemented

            // Make sure no one is blocked
            OnStatusEffectChanged?.Invoke();

            // Optionally, notify all domains unblocked if your system needs it
            // (Rare, usually effects handle unblocking individually)

            Debug.Log($"[{ScriptName}] CleanupSystem called, all effects cleared.");
        }
    }
}