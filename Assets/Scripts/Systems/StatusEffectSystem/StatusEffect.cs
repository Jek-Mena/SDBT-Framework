using System;
using UnityEngine;
using Utils;

namespace Systems.StatusEffectSystem
{
    public class StatusEffect
    {
        public string Id { get; private set; }
        
        private string _customName;     // Optional: override for subclasses, or set for instances
        public virtual string Name => !string.IsNullOrEmpty(_customName) ? _customName : GetType().Name;
        public void SetCustomName(string customName) => _customName = customName;

        public string Source; // Where this effect came from (node, BT, system, etc)
        public float Duration;
        public float TimeApplied;
        public string[] Domains;  // Which domains this effect blocks/modifies e.g. [BlockedDomain.Movement, BlockedDomain.Attack]
        
        public EffectMultipliers Multipliers = new EffectMultipliers();

        public StatusEffect()
        {
            Id = this.GetGuid().ToString(); 
        }

        public StatusEffect(string customName, string[] domains, float duration = 0f, string source = null)
            : this()
        {
            _customName = customName;
            Domains = domains ?? Array.Empty<string>();
            Duration = duration;
            Source = source;
            TimeApplied = Time.time;
        }
        
        public bool IsActive() => Time.time < TimeApplied + Duration;
        public bool AffectsDomain(string domain) => Array.Exists(Domains, d => d == domain);
    
        // Expose remaining duration for overlay clarity
        public float RemainingDuration => Mathf.Max(0, (TimeApplied + Duration) - Time.time);
    }
}