using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

public static class BtProfileResolver
{
    /// <summary>
    /// Scans all node configs in the tree, and for each key ending with 'Profile', tries to resolve the appropriate profile
    /// dictionary from the blackboard and injects the resolved profile data as 'resolved{ProfileKey}'.
    /// Example: { "targetProfile": "ChaseTarget" } will add { "resolvedTargetProfile": ... }
    /// </summary>
    public static void ResolveAllProfiles(JObject node, Blackboard blackboard)
    {
        // Map config key (e.g. "targetProfile") to blackboard dictionary property
        var profileSources = new Dictionary<string, Func<Blackboard, IDictionary<string, object>>>
        {
            { CoreKeys.Profiles.Targeting, bb => bb.TargetingProfiles?.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value) },
            { CoreKeys.Profiles.Movement, bb => bb.MovementProfiles?.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value) },
            // Extend here for more profile systems, e.g. movementProfile, attackProfile, etc.
        };

        ResolveProfilesRecursive(node, blackboard, profileSources);
    }

    /// <summary>
    /// Resolves profiles specified in the given JSON node using the provided blackboard and profile sources.
    /// For each profile key, attempts to resolve the profile from the blackboard dictionary and store the resolved profile in the node configuration.
    /// If the resolution fails, an exception is thrown. Also processes child nodes recursively.
    /// </summary>
    /// <param name="node">The JSON object representing the node configuration to resolve profiles for.</param>
    /// <param name="blackboard">The blackboard containing runtime data, including profile dictionaries.</param>
    /// <param name="profileSources">A dictionary mapping profile keys to functions that retrieve the associated profile dictionaries from the blackboard.</param>
    /// <exception cref="Exception">Thrown when a profile cannot be resolved for a specified key in the JSON node configuration.</exception>
    private static void ResolveProfilesRecursive(JObject node, Blackboard blackboard,
        Dictionary<string, Func<Blackboard, IDictionary<string, object>>> profileSources)
    {
        // Always work on config, or fall back to the node itself
        var config = node[CoreKeys.Config] as JObject ?? node;

        foreach (var profileKey in profileSources.Keys)
        {
            if (config.TryGetValue(profileKey, out var keyToken))
            {
                var key = keyToken.ToString();
                var profileDict = profileSources[profileKey](blackboard);

                if (profileDict != null && profileDict.TryGetValue(key, out var resolvedProfile))
                {
                    // Set "resolved{ProfileKey}" in config (e.g. resolvedTargetProfile)
                    var resolvedKey = $"{CoreKeys.ResolvedProfiles.Resolved}{char.ToUpper(profileKey[0])}{profileKey.Substring(1)}";
                    config[resolvedKey] = JToken.FromObject(resolvedProfile);
                }
                else
                {
                    throw new Exception($"[BtProfileResolver] Could not resolve profile '{key}' for key '{profileKey}' in node config.");
                }
            }
        }

        // Recursively process children nodes
        if (node.TryGetValue(CoreKeys.Children, out var childrenToken) && childrenToken is JArray children)
        {
            foreach (var child in children.OfType<JObject>())
            {
                ResolveProfilesRecursive(child, blackboard, profileSources);
            }
        }
    }
}