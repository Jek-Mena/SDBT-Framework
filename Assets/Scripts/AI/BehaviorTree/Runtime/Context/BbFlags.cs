using System;

namespace AI.BehaviorTree.Runtime.Context
{
    [Flags]
    public enum BbFlags : byte
    {
        HasFearStimuli = 1 << 0,
        // add more as needed…
    }
}