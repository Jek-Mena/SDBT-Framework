using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class BtLoader
{
    private const string TreePathPrefix = "Data/BTs/";

    public static JToken LoadJsonTreeBtId(string treeId)
    {
        var asset = Resources.Load<TextAsset>($"{TreePathPrefix}{treeId}");
        if (asset == null)
            throw new Exception($"BT tree JSON not found at: Resources/{TreePathPrefix}{treeId}.json");

        return JToken.Parse(asset.text);
    }

    public static void ApplyAll(GameObject entity, JObject configRoot)
    {
        // 1) Group plugins by phase and sort phases
        var byPhase = FluentPluginRegistry.All
            .GroupBy(m => m.Phase)
            .OrderBy(g => g.Key);

        foreach (var group in byPhase)
        {
            var sorted = TopoSort(
                group,
                meta => new HashSet<Type>(meta.DependsOn),
                meta => meta.PluginType
            ).ToList();

            // Move BtLoadTreePlugin to the end
            var reordered = sorted
                .Where(m => m.PluginType != typeof(BtLoadTreePlugin))
                .Concat(sorted.Where(m => m.PluginType == typeof(BtLoadTreePlugin)))
                .ToList();

            foreach (var meta in reordered)
            {
                var keyName = meta.Key.ToString();
                if (!configRoot.TryGetValue(keyName, out var token))
                {
                    Debug.LogError($"[BtLoader] Missing config for plugin '{keyName}'.");
                    throw new Exception($"[BtLoader] Missing config for plugin '{keyName}'.");
                }

                var @params = token as JObject;
                var plugin = (BasePlugin)Activator.CreateInstance(meta.PluginType);
                plugin.Apply(entity, @params);
            }
        }

        /*foreach (var group in byPhase)
        {
            // 2) Topo-sort withint he same phase by DependsOn
            var sorted = TopoSort(
                group,
                meta => new HashSet<Type>(meta.DependsOn),
                meta => meta.PluginType
            );

            // 3) Instantiate & Apply
            foreach (var meta in sorted)
            {
                // use the enum key (compile-safe) rather than Type.Name
                var keyName = meta.Key.ToString();
                if (!configRoot.TryGetValue(keyName, out var token))
                {
                    Debug.LogError($"[BtLoader] Missing config for plugin '{keyName}'.");
                    throw new Exception($"[BtLoader] Missing config for plugin '{keyName}'.");
                }
                var @params = token as JObject;

                var plugin = (BasePlugin)Activator.CreateInstance(meta.PluginType);
                plugin.Apply(entity, @params);
            }
        }*/
    }

    // Kahn’s algorithm, but keyed on Type instead of string:
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
            var u = queue.Dequeue();
            result.Add(u);
            foreach (var v in all.Where(i => getDeps(i).Contains(getKey(u))))
            {
                if (--inDegree[v] == 0)
                    queue.Enqueue(v);
            }
        }

        if (result.Count != all.Count)
            throw new Exception("Cyclic plugin dependency detected");

        return result;
    }
}