using System;
using System.Collections.Generic;
using UnityEngine;

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
        if (!BtValidator.Require(context)
                .Children(_children)
                .Check(out var error)
            )
        {
            Debug.Log(error);
            return BtStatus.Failure;
        }
        
        var anyRunning = false;
        var anySuccess = false;
        var anyFailure = false;
        var allSuccess = true;
        var allFailure = true;

        foreach (var child in _children)
        {
            var status = child.Tick(context);

            switch (status)
            {
                case BtStatus.Running:
                    anyRunning = true;
                    allSuccess = false;
                    allFailure = false;
                    break;
                case BtStatus.Success:
                    anySuccess = true;
                    allFailure = false;
                    break;
                case BtStatus.Failure:
                    anyFailure = true;
                    allSuccess = false;
                    break;
            }
        }

        return _exitCondition switch
        {
            ParallelExitCondition.FirstSuccess => anySuccess ? BtStatus.Success :
                anyRunning ? BtStatus.Running :
                BtStatus.Failure,

            ParallelExitCondition.FirstFailure => anyFailure ? BtStatus.Failure :
                anyRunning ? BtStatus.Running :
                BtStatus.Success,

            ParallelExitCondition.AllSuccess => allSuccess ? BtStatus.Success :
                anyRunning ? BtStatus.Running :
                BtStatus.Failure,

            ParallelExitCondition.AllFailure => allFailure ? BtStatus.Failure :
                anyRunning ? BtStatus.Running :
                BtStatus.Success,

            _ => throw new ArgumentOutOfRangeException(nameof(_exitCondition),
                "[BtParallelNode] Unknown or unsupported exit condition.")
        };
    }
}