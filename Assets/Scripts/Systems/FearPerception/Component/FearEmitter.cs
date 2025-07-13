using System;
using Systems.FearPerception.Component;
using UnityEngine;

public class FearEmitter : MonoBehaviour
{
    /// <summary>
    /// Fired when a new emitter is created, so the manager can register it.
    /// </summary>
    public static event Action<FearEmitter> OnEmitterCreated;

    [Header("Stimulus Settings")]
    public float strength = 1f;
    public float radius = 5f;
    public float effectDuration = 3f;
    [Tooltip("Who/what caused this fear stimulus? (Optional)")]
    public GameObject Source; // Can be attacker, environment, player, etc.

    private void Awake()
    {
        // Notify manager that we've been created (for runtime registration)
        OnEmitterCreated?.Invoke(this);       
    }

    private void OnDestroy()
    {
        if (FearStimulusManager.Instance)
            FearStimulusManager.Instance.Unregister(this);
    }
    
    /// <summary>
    /// Provides current stimulus data for querying.
    /// </summary>
    public FearStimulus? GetStimulus()
    {
        return new FearStimulus
        {
            Position = transform.position,
            Strength = strength,
            Radius = radius,
            EffectDuration = effectDuration,
            Source = Source ?? gameObject // Default to self if not set
        };
    }
    
    // --- DEBUG VISUALIZATION ---
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.25f); // Orange, semi-transparent
        Gizmos.DrawSphere(transform.position, radius); // Visualize radius
        Gizmos.color = Color.red; // Stronger color for the outline
        Gizmos.DrawWireSphere(transform.position, radius);

        // (Optional) Draw strength as a label
#if UNITY_EDITOR
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * (radius + 0.5f),
            $"Fear {strength:F2}");
#endif
    }
}