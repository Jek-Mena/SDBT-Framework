namespace AI.BehaviorTree.Nodes.Perception
{
    public interface IPerceptionSink
    {
        // Non-alloc path for hot sensors
        void Enqueue(ref StimulusEvent evt);
    }
}