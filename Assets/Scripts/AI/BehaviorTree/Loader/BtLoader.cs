using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class BtLoader
{
    public static void ApplyAll(GameObject entity, JObject jObject)
    {
        var controller = entity.RequireComponent<BtController>();

        // Ensure blackboard exist BEFORE running any plugins
        if (controller.Blackboard == null)
            throw new System.Exception($"[BtLoader] Blackboard missing on '{entity.name}' when applying plugins. You must build context before calling ApplyAll.");
        
        var components = jObject[CoreKeys.Components] as JArray;
        if (components == null)
        {
            Debug.LogError($"[BtLoader] No '{CoreKeys.Components}' array found in {entity.name}.");
            return;
        }

        // Group plugins by phase and sort phases
        var byPhase = PluginMetadataStore.RegisteredPlugins
            .GroupBy(m => m.ExecutionPhase)
            .OrderBy(g => g.Key);

        foreach (var group in byPhase)
        {
            var sorted = TopoSort(
                group,
                meta => new HashSet<Type>(meta.DependsOn),
                meta => meta.PluginType
            );

            foreach (var meta in sorted)
            {
                var matchingComponent = components
                    .OfType<JObject>()
                    .FirstOrDefault(c => c[CoreKeys.Plugin]?.ToString() == meta.PluginKey);

                if (matchingComponent == null)
                {
                    Debug.LogError($"[BtLoader] Missing plugin! '{meta.PluginKey}' � not present in 'components' list.");
                    continue;
                }

                var pluginParams = matchingComponent[CoreKeys.Params] as JObject ?? new JObject();
                var plugin = (BasePlugin)Activator.CreateInstance(meta.PluginType);
                plugin.Apply(entity, pluginParams);
            }
        }
    }

    // Kahn�s algorithm (for dependency ordering)
    private static List<T> TopoSort<T>(
        IEnumerable<T> items,
        Func<T, HashSet<Type>> getDeps,
        Func<T, Type> getKey
    )
    {
        var all = items.ToList();
        var inDegree = all.ToDictionary(i => i, _ => 0);
        var keyToItem = all.ToDictionary(getKey, i => i);

        // compute in-degrees
        foreach (var item in all)
        foreach (var dep in getDeps(item))
            if (keyToItem.ContainsKey(dep))
                inDegree[item]++;

        // Ebqyeye zero-degree nodes
        var queue = new Queue<T>(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
        var result = new List<T>();

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            result.Add(node);

            foreach (var dependent in all.Where(i => getDeps(i).Contains(getKey(node))))
            {
                if (--inDegree[dependent] == 0)
                    queue.Enqueue(dependent);
            }
        }

        if (result.Count != all.Count)
            throw new Exception("[BtLoader] Cyclic plugin dependency detected");

        return result;
    }
}