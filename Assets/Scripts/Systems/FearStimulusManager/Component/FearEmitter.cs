using UnityEngine;

public class FearEmitter : MonoBehaviour
{
    public float strength = 1f;
    public float radius = 5f;
    public float effectDuration = 3f;
    
    private void OnEnable()
    {
        FearStimulusManager.Register(new FearStimulus(transform.position, strength, radius, effectDuration, gameObject));
    }

    private void OnDisable()
    {
        FearStimulusManager.ClearAll(); // For prototype only
    }

    private void Update()
    {
        // For moving emitters, you’d want to update the stimulus in manager.
        // Prototype: Remove all and re-register each frame (not efficient, but clear for demo)
        FearStimulusManager.ClearAll();
        FearStimulusManager.Register(new FearStimulus(transform.position, strength, radius, effectDuration, gameObject));
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