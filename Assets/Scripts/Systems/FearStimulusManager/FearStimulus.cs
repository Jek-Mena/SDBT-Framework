using UnityEngine;

/// <summary>
/// [2025-06-27]
/// Represents a single source of fear in the world, with position, strength, area of effect, duration, and source object.
/// Agents within Radius are affected, with effect strength typically decreasing with distance.
/// </summary>
public struct FearStimulus
{
    /// <summary>
    /// World position of the fear emitter.
    /// </summary>
    public Vector3 Position;
    /// <summary>
    /// Maximum intensity of the fear effect at the center.
    /// </summary>
    public float Strength;
    /// <summary>
    /// Maximum distance from Position at which this stimulus can affect agents (area of effect).
    /// </summary>
    public float Radius;
    /// <summary>
    /// Duration (in seconds) for which this stimulus remains active.
    /// </summary>
    public float Duration;
    /// <summary>
    /// The GameObject that emitted this stimulus (e.g., enemy, player, explosion).
    /// </summary>
    public GameObject Source;
    
    public FearStimulus(Vector3 position, float strength, float radius, float duration, GameObject source)
    {
        Position = position;
        Strength = strength;
        Radius = radius;
        Duration = duration;
        Source = source;
    }
}