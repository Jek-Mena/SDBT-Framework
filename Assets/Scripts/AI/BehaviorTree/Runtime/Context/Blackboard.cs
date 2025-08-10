using System.Collections.Generic;
using Loader;
using UnityEngine;

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
        
        public static GameObject GetEntity(int id)
        {
            return EntityRegistry.TryGetValue(id, out var go) && go != null ? go : null;
        }
        
        public static void CleanupRegistry()
        {
            var dead = EntityRegistry.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key).ToList();
            foreach (var id in dead) EntityRegistry.Remove(id);
        }
        
        // ───────────────
        // Dynamic Key-Value Context Store
        // Only for non-core, optional extensions; never use for primary context fields.
        // ───────────────

        private readonly Dictionary<string, object> _dataDictionary = new();
        private string _btSessionId;

        /// <summary>
        /// Dynamically registers a runtime value with the blackboard.
        /// Useful for injecting settings, services, or tools without altering the main schema.
        /// </summary>
        public void Set<T>(string key, T value)
        {
            BlackboardMigration.MigrateSet(this, key, value); // write into Data when key is known
            _dataDictionary[key] = value; // keep legacy path alive
        }

        /// <summary>
        /// Retrieves a previously stored dynamic value.
        /// Logs a warning if the key is missing.
        /// </summary>
        public T Get<T>(string key, T defaultValue = default)
        {
            if (_dataDictionary.TryGetValue(key, out var value))
                return (T)value;

            Debug.LogWarning($"[Blackboard] Missing key '{key}' of type {typeof(T).Name}, returning default value '{defaultValue}'.");
            return defaultValue; // Use the user-supplied default!
        }

        /// <summary>
        /// Tries to retrieve a value safely with type checking.
        /// Returns true if the value was found and matched the requested type.
        /// </summary>
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
        
        /// <summary>
        /// Removes a previously stored dynamic value. Returns true if removed; false if not present.
        /// </summary>
        public bool Remove(string key)
        {
            return _dataDictionary.Remove(key);
        }

        // ───────────────
        // End Dynamic Key-Value Context Store
        // ───────────────

        /// <summary>
        /// Dumps the dynamic dictionary for debugging or inspection.
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> DumpDynamic() => _dataDictionary;
    }
}
