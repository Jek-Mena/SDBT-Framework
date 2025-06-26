using UnityEngine;

public class StatusEffect
{
    private string _customName;     // Optional: override for subclasses, or set for instances
    public virtual string Name => !string.IsNullOrEmpty(_customName) ? _customName : GetType().Name;
    public void SetCustomName(string customName) => _customName = customName;

    public string Source;
    public float Duration;
    public float TimeApplied;
    public string[] Domains;  // e.g. [BlockedDomain.Movement, BlockedDomain.Attack]
    // ...plus priority/stack/metadata as needed
    // For full feature parity with your ModifierStack/Meta system, you can swap out the logic to use your existing ModifierStack<StatusEffect>

    public EffectMultipliers Multipliers = new EffectMultipliers();
    public bool IsActive() => Time.time < TimeApplied + Duration;
    public bool AffectsDomain(string domain) => System.Array.Exists(Domains, d => d == domain);
    
    // Expose remaining duration for overlay clarity
    public float RemainingDuration => Mathf.Max(0, (TimeApplied + Duration) - Time.time);
}