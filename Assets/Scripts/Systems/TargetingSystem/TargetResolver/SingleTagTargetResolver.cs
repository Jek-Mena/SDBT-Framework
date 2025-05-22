using UnityEngine;

public class SingleTagTargetResolver : ITargetResolver
{
    private Transform _cachedPlayer;
    public Transform ResolveTarget(GameObject self, TargetingData data)
    {
        if (string.IsNullOrEmpty(data.TargetTag))
        {
            Debug.LogError("[SingleTagTargetResolver] TargetTag is null or empty!", self);
            return null;
        }

        if (_cachedPlayer == null || !_cachedPlayer.gameObject.activeInHierarchy)
        {
            var go = GameObject.FindGameObjectWithTag(data.TargetTag);
            if (go == null)
            {
                Debug.LogError($"[SingleTagTargetResolver] No GameObject found with tag '{data.TargetTag}'!", self);
                return null;
            }
            if (go == self)
            {
                Debug.LogError($"[SingleTagTargetResolver] Found self as target! Tag: '{data.TargetTag}'", self);
                return null;
            }
            _cachedPlayer = go.transform;
        }

        if (_cachedPlayer == null || !_cachedPlayer.gameObject.activeInHierarchy)
        {
            Debug.LogError("[SingleTagTargetResolver] Cached target is invalid or inactive!", self);
            return null;
        }

        return _cachedPlayer;
    }
}