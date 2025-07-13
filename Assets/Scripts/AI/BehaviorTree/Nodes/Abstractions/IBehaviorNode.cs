using System;
using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Runtime.Context;

public interface IBehaviorNode
{
    // Intent in -> Tick -> Status out.
    BtStatus Tick(BtContext context);
    void Reset(BtContext context);
    IEnumerable<IBehaviorNode> GetChildren { get; } // Expose children if any
    // For display/debug
    BtStatus LastStatus { get; }
    string DisplayName { get;  }
}