using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModifierStack<T>
{
    private readonly List<IModifier<T>> _modifiers = new();

    public void Add(IModifier<T> modifier)
    {
        if (modifier == null)
        {
            Debug.LogError($"[ModifierStack] Attempted to add a null modifier to {typeof(T).Name}");
            return;
        }

        if (!modifier.CanStack)
        {
            // Remove exactly ONE instance from the same source
            var existing = _modifiers.FirstOrDefault(m => m.Source == modifier.Source);
            if (existing != null)
                _modifiers.Remove(existing);
        }
        else
        {
            var sameSource = _modifiers.Where(m => m.Source == modifier.Source).ToList();

            if (sameSource.Count >= modifier.MaxStacks)
            {
                // Replace the one you want (e.g., lowest priority or oldest)
                var toReplace = sameSource.OrderBy(m => m.Priority).First();
                _modifiers.Remove(toReplace);
            }
        }

        _modifiers.Add(modifier);
    }

    public T Apply(T baseValue)
    {
        var result = baseValue;

        if (result == null)
            Debug.LogError($"Null {baseValue}");

        foreach (var mod in _modifiers.OrderByDescending(m => m.Priority))
        {
            if (mod == null)
            {
                Debug.LogError($"[ModifierStack] Null modifier found in stack of {typeof(T).Name}. Skipping.");
                continue;
            }

            result = mod.Apply(result);

            if (result == null)
            {
                Debug.LogError($"Modifier {mod.Source} returned null result in Apply(). This is illegal.");
                throw new Exception("Modifier returned null in Apply()");
            }
        }

        return result;
    }

    public T ApplySafe()
    {
        var baseValue = typeof(T).IsValueType ? System.Activator.CreateInstance<T>() : default;
        return Apply(baseValue);
    }

    public void RemoveAllFromSource(string source)
    {
        // For purge-style behavior
        _modifiers.RemoveAll(m => m.Source == source);
    }

    public void RemoveOneFromSource(string source)
    {
        // For stack replacement
        var toRemove = _modifiers
            .Where(m => m.Source == source)
            .OrderBy(m => m.Priority) // or add timestamp & sort by oldest
            .FirstOrDefault();

        if (toRemove != null)
            _modifiers.Remove(toRemove);
    }

    public void Remove(IModifier<T> modifier)
    {
        // Useful if tracking modifiers individually by instance.
        _modifiers.Remove(modifier);
    }
}