using System;

namespace AI.BehaviorTree.Runtime.Context
{
    /// <summary>
    /// Strongly-typed key for blackboard access. Prevents string allocations and type mismatches.
    /// Use static readonly instances for performance and consistency.
    /// </summary>
    /// <typeparam name="T">The type of value this key represents</typeparam>
    public readonly struct Key<T> : IEquatable<Key<T>>
    {
        private readonly int _id;
        private readonly string _debugName;

        public Key(int id, string debugName = null)
        {
            _id = id;
            _debugName = debugName ?? $"Key<{typeof(T).Name}>({id})";
        }

        public int Id => _id;
        public string DebugName => _debugName;

        public bool Equals(Key<T> other) => _id == other._id;
        public override bool Equals(object obj) => obj is Key<T> other && Equals(other);
        public override int GetHashCode() => _id;
        public override string ToString() => _debugName;

        public static bool operator ==(Key<T> left, Key<T> right) => left.Equals(right);
        public static bool operator !=(Key<T> left, Key<T> right) => !left.Equals(right);
    }
}