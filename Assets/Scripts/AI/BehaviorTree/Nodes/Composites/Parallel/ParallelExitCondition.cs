/// <summary>
/// Defines the conditions under which a parallel node will exit or return a result.
/// Determines how the behavior of the parallel node is evaluated based on child node outcomes.
/// The possible exit conditions are:
/// - FirstSuccess: The node returns Success as soon as any child returns Success.
/// - FirstFailure: The node returns Failure as soon as any child returns Failure.
/// - AllSuccess: The node returns Success only when all children return Success.
/// - AllFailure: The node returns Failure only when all children return Failure.
/// These conditions dictate the evaluation logic across all child nodes and control how the overall
/// behavior tree proceeds based on the aggregated results of its children.
///
/// Additionally, ": byte" makse the enum non-extensible at runtime.
/// </summary>
public enum ParallelExitCondition : byte
{
    FirstSuccess = 0,
    FirstFailure = 1,
    AllSuccess = 2,
    AllFailure = 3
}