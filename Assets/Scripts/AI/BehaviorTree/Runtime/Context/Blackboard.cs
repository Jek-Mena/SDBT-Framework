using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace AI.BehaviorTree.Runtime.Context
{
    /// <summary>
    /// Runtime container for AI-specific data used across behavior tree nodes.
    /// Holds both strongly-typed fields for common systems and a dynamic key-value store for flexible extensions.
    ///
    /// [2025-06-18] Migrated to Profile System: profile data is accessed via ProfileDictionaries only.
    /// 
    /// [2025-06-17 ARCHITECTURE NOTE]
    /// All core runtime data for AI entities must flow through Blackboard.
    /// - Dynamic data (_data/Set/Get/TryGet) is only for optional or extension systems, never core.
    /// - Each field must have a single owner: document the context builder module or system responsible.
    /// - No field may be mutated by random scripts, only via context modules, the pipeline, or authorized plugins.
    /// - If adding a new field, update DumpContents and document owner.
    /// </summary>
    public class Blackboard
    {
        private BlackboardData _data = BlackboardData.CreateDefaults(); // use defaults
        public BlackboardData Data { get => _data; set => _data = value; }
        public ref BlackboardData DataRef => ref _data;

        private const string ScriptName = nameof(Blackboard);
        private string _btSessionId;
        
        // Simple entity registry
        private static readonly Dictionary<int, GameObject> EntityRegistry = new();
        
        public static GameObject GetEntity(int id)
        {
            return EntityRegistry.TryGetValue(id, out var go) && go != null ? go : null;
        }
        
        public static int RegisterEntity(GameObject go)
        {
            if (go == null) return 0;
            var id = go.GetInstanceID();
            EntityRegistry[id] = go;
            return id;
        }
        
        public static void CleanupRegistry()
        {
            var dead = EntityRegistry.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key).ToList();
            foreach (var id in dead) EntityRegistry.Remove(id);
        }
        
        private readonly Dictionary<int, object> _dynamic = new();
        // NEW: typed API (preferred)
        public void SetTypedAPI<T>(BbKey<T> key, T value) => _dynamic[key.Id] = value;

        public bool TryGet<T>(BbKey<T> key, out T value)
        {
            if (_dynamic.TryGetValue(key.Id, out var raw) && raw is T t)
            { value = t; return true; }
            value = default; return false;
        }

        public T Get<T>(BbKey<T> key, T defaultValue = default)
        {
            return _dynamic.TryGetValue(key.Id, out var raw) && raw is T t ? t : defaultValue;
        }
        
        /// <summary>
        /// Get the long-lived pooled List<T> backing a BbKey&lt;IReadOnlyList&lt;T&gt;&gt;.
        /// Allocates once (from pool), then reused forever for this agent.
        /// Capacity is clamped to a fixed max and never allowed to grow.
        /// </summary>
        public List<T> GetListForWriteFixed<T>(BbKey<IReadOnlyList<T>> key, int fixedCapacity)
        {
            if (_dynamic.TryGetValue(key.Id, out var raw) && raw is List<T> list) return list;
            var newList = ListPool<T>.Get(); // pooled allocation (once)
            if (newList.Capacity < fixedCapacity) newList.Capacity = fixedCapacity;
            _dynamic[key.Id] = newList;  // store the STABLE reference
            return newList;
        }
        
        /// <summary>Read-only view of the long-lived list. Zero alloc.</summary>
        public IReadOnlyList<T> GetListForRead<T>(BbKey<IReadOnlyList<T>> key)
        {
            if (_dynamic.TryGetValue(key.Id, out var raw) && raw is List<T> list)
                return list;
            return System.Array.Empty<T>();
        }
        
        /// <summary>Release the pooled list when the agent is destroyed.</summary>
        public void ReleasePooledList<T>(BbKey<IReadOnlyList<T>> key)
        {
            if (_dynamic.TryGetValue(key.Id, out var raw) && raw is List<T> list)
            {
                list.Clear();
                ListPool<T>.Release(list);
                _dynamic.Remove(key.Id);
            }
        }
        
        public bool Remove<T>(BbKey<T> key) => _dynamic.Remove(key.Id);
        
        // Legacy
        [System.Obsolete]
        private readonly Dictionary<string, object> _dataDictionary = new();

        [System.Obsolete("Use typed BbKey<T> API. Register keys and call Set(BbKey<T>, value).")]
        public void Set<T>(string key, T value)
        {
            _dataDictionary[key] = value; // keep legacy path alive
        }
        
        [System.Obsolete("Use typed BbKey<T> API.")]
        public T Get<T>(string key, T defaultValue = default)
        {
            if (_dataDictionary.TryGetValue(key, out var value))
                return (T)value;

            Debug.LogWarning($"[Blackboard] Missing key '{key}' of type {typeof(T).Name}, returning default value '{defaultValue}'.");
            return defaultValue; // Use the user-supplied default!
        }

        [System.Obsolete("Use typed BbKey<T> API.")]
        public bool TryGet<T>(string key, out T value)
        {
            if (_dataDictionary.TryGetValue(key, out var raw) && raw is T cast)
            {
                value = cast;
                return true;
            }

            value = default;
            return false; 
        }
        
        [System.Obsolete("Use typed BbKey<T> API. I think I need to create something similar or refactor")]
        public IEnumerable<T> GetAll<T>()
        {
            // Materialize to a List to avoid mutation-while-iteration issues
            var snapshot = new List<object>(_dataDictionary.Values);
            foreach (var obj in snapshot)
            {
                if (obj is T tObj)
                    yield return tObj;
            }
        }
    }
}
