using System.Collections.Generic;

public class StatBlock
{
    public float Movement = 1.0f;
    public float Attack = 1.0f;
    public float Armor = 1.0f;

    // Optional: If you want custom/rare stats.
    public Dictionary<string, float> Custom = new();

    public void SetCustom(string key, float value) => Custom[key] = value;
    public float GetCustom(string key) => Custom.TryGetValue(key, out var v) ? v : 1.0f;
    public void Reset()
    {
        Movement = 1.0f;
        Attack = 1.0f;
        Armor = 1.0f;
        Custom.Clear();
    }

    // Aggregation: multiply by another stat block (for stacking)
    public void MultiplyWith(StatBlock other)
    {
        Movement *= other.Movement;
        Attack *= other.Attack;
        Armor *= other.Armor;
        foreach (var kvp in other.Custom)
            SetCustom(kvp.Key, GetCustom(kvp.Key) * kvp.Value);
    }
}