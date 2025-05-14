using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class TimerKeyBuilder
{
    /// <summary>
    /// Generate a runtime-safe timer key from a user-provided config string,
    /// scoped to the blackboard (and optionally randomized).
    /// </summary>
    public static string Build(JObject config, string defaultPrefix, Blackboard blackboard, string context)
    {
        var raw = config.Value<string>(TimedExecutionKeys.Json.Label);

        if (!string.IsNullOrWhiteSpace(raw))
        {
            var scoped = $"{raw}:{blackboard.GetHashCode()}";
            Debug.Log($"[TimerKeyBuilder] Using key '{raw}', scoped as '{scoped}' (context: {context})");
            return scoped;
        }

        var fallback = $"{defaultPrefix}:{blackboard.GetHashCode()}:{Guid.NewGuid()}";
        Debug.LogWarning($"[TimerKeyBuilder] No key provided — generated fallback '{fallback}' (context: {context})");
        return fallback;
    }
}