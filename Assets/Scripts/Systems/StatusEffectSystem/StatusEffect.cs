using UnityEngine;

public class StatusEffect
{
    public string Source;
    public float Duration;
    public float TimeApplied;
    public string[] Domains;  // e.g. [BlockedDomain.Movement, BlockedDomain.Attack]
    // ...plus priority/stack/metadata as needed
    // For full feature parity with your ModifierStack/Meta system, you can swap out the logic to use your existing ModifierStack<StatusEffect>
    
    public bool IsActive() => Time.time < TimeApplied + Duration;
    public bool AffectsDomain(string domain) => System.Array.Exists(Domains, d => d == domain);
}