
using System.Linq;
using System.Text;

/// <summary>
/// [2025-06-26]
/// Provides debug tools for behavior trees, including methods for generating
/// a textual representation of a behavior tree for debugging purposes.
/// </summary>
public class BtDebugTools
{
    public static void DumpTree(IBehaviorNode node, StringBuilder stringBuilder, int depth = 0)
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
            DumpTree(wrapper.GetChildren.FirstOrDefault(), stringBuilder, depth); // Skip the wrapper node itself
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
                DumpTree(child, stringBuilder, depth + 1);
        }
    }
}
