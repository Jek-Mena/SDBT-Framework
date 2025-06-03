using System;

public interface IBehaviorNode
{
    BtStatus Tick(BtContext context);

    // TEMPORARY: Legacy fallback
    //[Obsolete("Use Tick(BtContext) instead.")]
    //BtStatus Tick(BtController controller) => Tick(new BtContext(controller));
}
