
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// [2025-06-26]
/// Provides debug tools for behavior trees, including methods for generating
/// a textual representation of a behavior tree for debugging purposes.
/// </summary>
public class BtDebugTools
{
    /// <summary>
    /// Collects all nodes on *any* currently Running path (including all active parallel branches).
    /// </summary>
    public static void BuildActivePaths(IBehaviorNode node, HashSet<IBehaviorNode> path)
    {
        if (node == null) return;
        path.Add(node);
        
        var children = node.GetChildren ?? Enumerable.Empty<IBehaviorNode>();
        // If node is running, add all running children (parallel support)
        if (node.LastStatus == BtStatus.Running && children.Any())
        {
            foreach (var child in children)
            {
                if (child.LastStatus == BtStatus.Running)
                    BuildActivePaths(child, path);
            }
        }
        // If leaf, done (already added above)
    }

    /// <summary>
    /// Dumps the tree, highlighting all nodes on active (Running) paths (supports Parallel).
    /// Non-active nodes are gray.
    /// </summary>
    public static void DumpTreeActivePaths(
        IBehaviorNode node,
        StringBuilder stringBuilder,
        int depth,
        HashSet<IBehaviorNode> activePath)
    {
        if (node == null) return;

        // === TODO / DESIGN NOTE ===
        // This skips BtLifecycleNode so wrappers don't appear in the overlay.
        // If you add more wrapper types, you'll need to extend this check (non-scalable, but fine for now).
        if (node is BtLifecycleNode wrapper)
        {
            DumpTreeActivePaths(wrapper.GetChildren.FirstOrDefault(), stringBuilder, depth, activePath);
            return;
        }

        var indent = new string(' ', depth * 2);
        var isActive = activePath.Contains(node);

        var openColor = isActive
            ? node.LastStatus switch
            {
                BtStatus.Running => "<color=yellow>",
                BtStatus.Success => "<color=green>",
                BtStatus.Failure => "<color=red>",
                BtStatus.Idle    => "<color=gray>",
                BtStatus.Warning => "<color=orange>",
                _ => "<color=white>"
            }
            : "<color=black>";
        var closeColor = "</color>";
        
        stringBuilder.AppendLine($"{indent}{openColor}{node.DisplayName}-[{node.LastStatus}]{closeColor}");

        foreach (var child in node.GetChildren ?? Enumerable.Empty<IBehaviorNode>())
            DumpTreeActivePaths(child, stringBuilder, depth + 1, activePath);
    }

    /// <summary>
    /// Generates a textual representation of the behavior tree, highlighting
    /// the last success status for all nodes in the tree.
    /// </summary>
    public static void DumpTreeLastSuccess(IBehaviorNode node, StringBuilder stringBuilder, int depth = 0)
    {
        if (node == null) return;
        var indent = new string(' ', depth * 2);

        // === TODO / DESIGN NOTE ===
        // [2025-06-26] This logic unwraps transparent node wrappers (like BtLifecycleNode) during debug dump,
        // so only the *meaningful* node appears in overlays—hides boilerplate decorator noise.
        //
        // PROBLEM: This is a type-specific hack. If you add more wrappers/decorators (e.g., Repeaters, Conditionals, etc.),
        // you'll have to keep updating this check—brittle and non-scalable.
        //
        // UPGRADE: Refactor all transparent wrappers to implement an `INodeWrapper` interface.
        // Then, replace this with a generic `while (node is INodeWrapper w) node = w.WrappedNode;` loop.
        // This will future-proof all debug/log/overlay tools and keep display logic DRY and extensible.
        //
        // For now: hack is acceptable for milestone/demo, but DO NOT SHIP LONG-TERM.
        // If this is a transparent wrapper, only dump its child
        if (node is BtLifecycleNode wrapper)
        {
            DumpTreeLastSuccess(wrapper.GetChildren.FirstOrDefault(), stringBuilder, depth); // Skip the wrapper node itself
            return;
        }
        
        var statusColor = node.LastStatus switch
        {
            BtStatus.Running => "<color=yellow>",
            BtStatus.Success => "<color=green>",
            BtStatus.Failure => "<color=red>",
            BtStatus.Idle => "<color=gray>",
            BtStatus.Warning => "<color=orange>",
            _ => "<color=white>"
        };
        var closeColor = "</color>";
        
        stringBuilder.AppendLine($"{indent}{statusColor}{node.DisplayName} [{node.LastStatus}]{closeColor}");
        
        // GetChildren should never be null—empty for leaf, but always safe
        if (node.GetChildren != null)
        {
            foreach (var child in node.GetChildren)
                DumpTreeLastSuccess(child, stringBuilder, depth + 1);
        }
    }
}
