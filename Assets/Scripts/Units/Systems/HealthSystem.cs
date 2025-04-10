using UnityEngine;

public class HealthSystem : MonoBehaviour, IRequire<IHealthAttributes>
{
    private IHealthAttributes _healthAttributes;

    private float _currentHealth;
    private float _maxHealth;
    private float _regen;

    public void Inject(IHealthAttributes dependency)
    {
        _healthAttributes = dependency;
        _currentHealth = _healthAttributes.MaxHealth;
        _maxHealth = _healthAttributes.MaxHealth;
        _regen = _healthAttributes.RegenRate;
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0)
            Die();
    }
    
    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
    }

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
}