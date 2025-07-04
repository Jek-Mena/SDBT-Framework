using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central manager for all FearEmitter sources in the scene.
/// Finds and tracks all emitters, supports dynamic registration for spawned emitters,
/// and provides a query API for perception modules.
/// </summary>
public class FearStimulusManager : MonoBehaviour
{
    public static FearStimulusManager Instance { get; private set; }
    private const string ScriptName = nameof(FearStimulusManager);
    private readonly HashSet<FearEmitter> _emitters = new();

    private void Awake()
    {
        // Singleton assignment
        if (!Instance) Instance = this;
        else if (Instance)
        {
            Debug.LogError($"[{ScriptName}] Multiple instances found, destroying duplicate!");
            Destroy(gameObject);
            return;
        }

        RegisterAllEmitters();
        
        // Listen for runtime emitter creation
        FearEmitter.OnEmitterCreated += Register;
    }

    /// <summary>
    /// Finds all FearEmitters in the scene and registers them.
    /// Call this on Awake or when resetting the system.
    /// </summary>
    public void RegisterAllEmitters()
    {
        var found = FindObjectsByType<FearEmitter>(sortMode: FindObjectsSortMode.InstanceID);
        foreach (var emitter in found)
            Register(emitter);
        
        Debug.Log($"[{ScriptName}] Registered {found.Length} emitters.");
    }

    /// <summary>
    /// Registers a new emitter (from initial scan or runtime event).
    /// </summary>
    public void Register(FearEmitter emitter)
    {
        if (!emitter) return;
        if(_emitters.Add(emitter))
            Debug.Log($"[{ScriptName}] Registered emitter {emitter.name}");
    }

    /// <summary>
    /// Unregisters an emitter (on destroy).
    /// </summary>
    public void Unregister(FearEmitter emitter)
    {
        if (!emitter) return;
        if (_emitters.Remove(emitter))
            Debug.Log($"[{ScriptName}] Unregistered emitter {emitter.name}");
    }
    
    /// <summary>
    /// Query all active fear stimuli within a given range of a position.
    /// </summary>
    public List<FearStimulus> Query(Vector3 position, float range)
    {
        var result = new List<FearStimulus>();
        foreach (var emitter in _emitters)
        {
            if (!emitter) continue;
            var stim = emitter.GetStimulus();
            if (!stim.HasValue) continue;
            var dist = Vector3.Distance(position, stim.Value.Position);
            if (dist <= range)
                result.Add(stim.Value);
        }
        return result;
    }
    
    public IEnumerable<FearEmitter> GetAllEmitters() => _emitters;
    
    private void OnDestroy()
    {
        if (Instance == this)
            FearEmitter.OnEmitterCreated -= Register;
    }
}