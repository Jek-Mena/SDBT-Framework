using System;
using System.Runtime.CompilerServices;

namespace Utils
{
    // TODO [2025-07-18]
    // Auto-purge GUIDs from destroyed objects, or extend this into a full runtime identity system
    public static class ObjectExtensions
    {
        private static readonly ConditionalWeakTable<object, GuidBox> Guids = new();

        public static Guid GetGuid(this object obj)
        {
            if (IsUnityNull(obj)) 
                throw new ArgumentNullException(nameof(obj), "Cannot assign GUID to destroyed or null Unity object.");
            
            return Guids.GetValue(obj, _ => new GuidBox()).Value;
        }

        public static void ResetRuntimeId(this object obj)
        {
            if (IsUnityNull(obj))
                throw new ArgumentNullException(nameof(obj), "Cannot reset GUID of destroyed or null Unity object.");

            if (Guids.TryGetValue(obj, out var box))
                box.Reset();
        }
        
        /// <summary>
        /// True if the object is null OR a destroyed UnityEngine.Object pretending to be null.
        /// </summary>
        private static bool IsUnityNull(object obj)
        {
            return obj == null || (obj is UnityEngine.Object uObj && uObj == null);
        }
    }
    
    internal class GuidBox
    {
        private Guid _guid = Guid.NewGuid();
        private readonly object _lock = new();

        public Guid Value
        {
            get
            {
                lock (_lock)
                    return _guid;
            }
            set
            {
                lock (_lock)
                    _guid = value;
            }
        }

        public void Reset()
        {
            lock (_lock)
                _guid = Guid.NewGuid();
        }
    }

}