using System.Collections.Generic;

/// <summary>
/// Parallel node that ticks all children every frame.
/// Returns:
/// - Failure: if ANY child returns Failure
/// - Running: if ANY child is Running (and none failed)
/// - Success: if ALL children return Success
///
/// ⚠️ This implementation is Success-on-ALL.
/// If even one child returns Running, the whole node stays Running.
/// If even one child fails, the whole thing fails immediately.
/// </summary>
public class BtParallelNode : IBehaviorNode
{
    private readonly List<IBehaviorNode> _children;

    public BtParallelNode(List<IBehaviorNode> children)
    {
        _children = children;
    }

    public BtStatus Tick(BtController controller)
    {
        var allSucceeded = true;
        var anyRunning = false;

        foreach (var child in _children)
        {
            var status = child.Tick(controller);

            if (status == BtStatus.Failure)
                return BtStatus.Failure;

            if (status == BtStatus.Running)
                anyRunning = true;

            if(status != BtStatus.Success)
                allSucceeded = false;
        }

        if (anyRunning) return BtStatus.Running;
        return allSucceeded ? BtStatus.Success : BtStatus.Running;
    }
}

// TODO: Extend BtParallelNode with support for configurable execution mode.
//
// Currently this node implements Parallel (Success-on-ALL):
// - Returns Failure immediately if ANY child fails
// - Returns Running if ANY child is still running
// - Returns Success ONLY if ALL children succeed
//
// 🔜 Planned Extension: Add support for Success-on-ANY (aka Fail-on-All)
//
// New config format:
// {
//     "type": "Parallel",
//     "config": {
//         "mode": "All" // or "Any"
//     },
//     "children": [ ... ]
// }
//
// Design plan:
// - Add enum ParallelExecutionMode { All, Any }
// - Pass mode via constructor
// - Adjust logic to:
//   - All: current implementation (fail on any failure)
//   - Any: succeed on first success, fail only if all fail
//
// Optional enhancements:
// - Timeout handling for runaway parallel nodes
// - Logging tick summary for debugging ("3 Success / 1 Running / 0 Failures")
// - Tracking which child triggered success/failure for visual debugging
