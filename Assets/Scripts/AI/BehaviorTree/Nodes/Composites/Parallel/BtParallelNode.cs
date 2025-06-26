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
/// - Implementation supports all standard exit conditions.
/// </summary>
public class BtParallelNode : IBehaviorNode
{
    private const string ScriptName = nameof(BtParallelNode);

    private BtStatus _lastStatus = BtStatus.Idle;
    public BtStatus LastStatus => _lastStatus;
    public string DisplayName => BtNodeDisplayName.Composite.Parallel;
    public IEnumerable<IBehaviorNode> GetChildren => _children;

    private readonly List<IBehaviorNode> _children;
    private readonly ParallelExitCondition _exitCondition;

    public BtParallelNode(List<IBehaviorNode> children, ParallelExitCondition exitCondition)
    {
        _children = children;
        _exitCondition = exitCondition;
    }

    public BtStatus Tick(BtContext context)
    {
        if (_children == null || _children.Count == 0)
        {
            Debug.LogError($"[{ScriptName}] No children found.");
            _lastStatus = BtStatus.Failure;
            return _lastStatus;
        }

        if (!BtValidator.Require(context)
                .Children(_children)
                .Check(out var error)
            )
        {
            Debug.LogError(error);
            _lastStatus = BtStatus.Failure;
            return _lastStatus;
        }

        var anyRunning = false;
        var anySuccess = false;
        var anyFailure = false;
        var allSuccess = true;
        var allFailure = true;

        for (int i = 0; i < _children.Count; i++)
        {
            Debug.Log($"[{ScriptName}] Ticking child {i}");
            var status = _children[i].Tick(context);

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

        switch (_exitCondition)
        {
            case ParallelExitCondition.FirstSuccess:
                _lastStatus = anySuccess ? BtStatus.Success
                               : anyRunning ? BtStatus.Running
                               : BtStatus.Failure;
                break;

            case ParallelExitCondition.FirstFailure:
                _lastStatus = anyFailure ? BtStatus.Failure
                               : anyRunning ? BtStatus.Running
                               : BtStatus.Success;
                break;

            case ParallelExitCondition.AllSuccess:
                _lastStatus = allSuccess ? BtStatus.Success
                               : anyRunning ? BtStatus.Running
                               : BtStatus.Failure;
                break;

            case ParallelExitCondition.AllFailure:
                _lastStatus = allFailure ? BtStatus.Failure
                               : anyRunning ? BtStatus.Running
                               : BtStatus.Success;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(_exitCondition),
                    $"[{ScriptName}] Unknown or unsupported exit condition.");
        }

        return _lastStatus;
    }
}
