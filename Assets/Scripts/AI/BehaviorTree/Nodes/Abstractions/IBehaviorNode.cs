using System;
using System.Collections.Generic;

public interface IBehaviorNode
{
    // Intent in -> Tick -> Status out.
    BtStatus Tick(BtContext context);
    BtStatus LastStatus { get; }
    string NodeName { get;  } // For display/debug
    IEnumerable<IBehaviorNode> GetChildren { get; } // Expose children if any
}
