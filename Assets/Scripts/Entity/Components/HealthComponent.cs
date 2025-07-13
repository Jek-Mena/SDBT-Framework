
using Systems.StatusEffectSystem.Component;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IUsesStatusEffectManager
{
    private const string ScriptName = nameof(HealthComponent);
    public float CurrentHealth { get; private set; }
    private HealthData _healthData;

    public void Initialize(HealthData healthData)
    {
        _healthData = healthData;
        CurrentHealth = _healthData.MaxHealth;
    }

    private void Update()
    {
        if (CurrentHealth < _healthData.MaxHealth)
        {
            CurrentHealth = Mathf.Min(_healthData.MaxHealth, CurrentHealth +_healthData.RegenRate * Time.deltaTime);
            // Optionally update blackboard here or fire event
        }
    }
    
    public void TakeDamage (float damage)
    {
        CurrentHealth -= damage;
        // Optionally update blackboard here or fire event
    }

    public void SetStatusEffectManager(StatusEffectManager manager)
    {
        Debug.Log($"[{ScriptName}] Setting up StatusEffectManager...");
    }

    public void OnDomainBlocked(string domain)
    {
        Debug.Log($"[{ScriptName}] Domained blocked...");
    }

    public void OnDomainUnblocked(string domain)
    {
        Debug.Log($"[{ScriptName}] Domained unblocked...");
    }
}
