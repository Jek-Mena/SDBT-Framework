using Assets.Scripts.Shared.AI;

public interface IBehaviorNode
{
    BtStatus Tick(BtController controller);
}
