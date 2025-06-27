using UnityEngine;

public class SingleTagTargetResolver : ITargetResolver
{
    private Transform _cachedPlayer;
    
    private const string ScriptName = nameof(SingleTagTargetResolver);
    public Transform ResolveTarget(GameObject self, TargetingData data)
    {
        if (string.IsNullOrEmpty(data.TargetTag))
        {
            Debug.LogError($"[{ScriptName}] TargetTag is null or empty!", self);
            return null;
        }

        if (!_cachedPlayer || !_cachedPlayer.gameObject.activeInHierarchy)
        {
            var go = GameObject.FindGameObjectWithTag(data.TargetTag);
            if (!go)
            {
                Debug.LogError($"[{ScriptName}] No GameObject found with tag '{data.TargetTag}'!", self);
                return null;
            }
            if (go == self)
            {
                Debug.LogError($"[{ScriptName}] Found self as target! Tag: '{data.TargetTag}'", self);
                return null;
            }
            _cachedPlayer = go.transform;
        }

        if (!_cachedPlayer || !_cachedPlayer.gameObject.activeInHierarchy)
        {
            Debug.LogError($"[{ScriptName}] Cached target is invalid or inactive!", self);
            return null;
        }

        return _cachedPlayer;
    }
}