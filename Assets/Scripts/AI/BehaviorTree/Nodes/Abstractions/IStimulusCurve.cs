namespace AI.BehaviorTree.Nodes.Abstractions
{
    public interface IStimulusCurve
    {
        float Evaluate(float stimulus);
    }
}