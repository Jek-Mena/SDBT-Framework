using System;
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
    private readonly ParallelExitCondition _exitCondition;

    public BtParallelNode(List<IBehaviorNode> children, ParallelExitCondition exitCondition)
    {
        _children = children;
        _exitCondition = exitCondition;
    }

    public BtStatus Tick(BtContext context)
    {
        var anyRunning = false;
        var anySuccess = false;
        var anyFailure = false;

        foreach (var child in _children)
        {
            var status = child.Tick(context);

            if (status == BtStatus.Running) anyRunning = true;
            if (status == BtStatus.Success) anySuccess = true;
            if (status == BtStatus.Failure) anyFailure = true;
        }

        switch (_exitCondition)
        {
            case ParallelExitCondition.FirstSuccess:
                if (anySuccess) return BtStatus.Success;
                if (anyRunning) return BtStatus.Running;
                return BtStatus.Failure;

            case ParallelExitCondition.FirstFailure:
                if (anyFailure) return BtStatus.Failure;
                if (anyRunning) return BtStatus.Running;
                return BtStatus.Success;

            // Extend for AllSuccess, AllFailure if needed
            default:
                throw new Exception("[BtParallelNode] Unknown or unsupported exit condition.");
        }
    }
}

public enum ParallelExitCondition
{
    FirstSuccess,
    FirstFailure,
    AllSuccess,
    AllFailure
}