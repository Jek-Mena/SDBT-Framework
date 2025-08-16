using System;
using System.Collections.Generic;

namespace AI.BehaviorTree.Runtime.Context
{
    internal static class BlackboardKeyRegistry
    {
        private static readonly Dictionary<string, (int id, Type type)> _byName = new();
        private static int _next = 1;

        public static BbKey<T> Register<T>(string name)
        {
            if (_byName.TryGetValue(name, out var entry))
            {
                if (entry.type != typeof(T))
                    throw new InvalidOperationException(
                        $"Blackboard key '{name}' already registered with type {entry.type.Name}, not {typeof(T).Name}.");
                return new BbKey<T>(entry.id, name);
            }
            var id = _next++;
            _byName[name] = (id, typeof(T));
            return new BbKey<T>(id, name);
        }

        public static int GetIdOrThrow<T>(string name)
        {
            if (!_byName.TryGetValue(name, out var entry))
                throw new KeyNotFoundException($"Blackboard key '{name}' is not registered.");
            if (entry.type != typeof(T))
                throw new InvalidOperationException(
                    $"Blackboard key '{name}' registered as {entry.type.Name} but requested as {typeof(T).Name}.");
            return entry.id;
        }
    }
}