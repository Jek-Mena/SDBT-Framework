
public class BehaviorLeafNode : IBehaviorNode
{
    private readonly IBehaviorNode _behavior;

    public BehaviorLeafNode(IBehaviorNode behavior)
    {
        _behavior = behavior;
    }

    public BtStatus Tick(BtController controller)
    {
        return _behavior.Tick(controller);
    }
}