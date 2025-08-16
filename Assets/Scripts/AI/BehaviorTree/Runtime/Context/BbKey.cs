namespace AI.BehaviorTree.Runtime.Context
{
    public readonly struct BbKey<T>
    {
        public readonly int Id;
        public readonly string Name; // for debug
        internal BbKey(int id, string name) { Id = id; Name = name; }
        public override string ToString() => Name;
    }

    public static class BbKey
    {
        public static BbKey<T> Create<T>(string name) => 
            BlackboardKeyRegistry.Register<T>(name);
    }
}