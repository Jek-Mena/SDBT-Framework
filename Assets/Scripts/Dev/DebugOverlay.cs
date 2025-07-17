using System.Collections.Generic;
using System.Text;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
using Systems.StatusEffectSystem.Component;
using UnityEngine;

/// <summary>
/// [2025-06-26]
/// Provides functionality to display and interact with a debug overlay within the Unity environment.
/// This component is used to aid in debugging and monitoring behavior tree execution using context-specific data.
///
/// 
/// </summary>
public class DebugOverlay : MonoBehaviour
{
    // TODO [2025-07-14] [DebugOverlay Refactor]:
    // Decouple debug info generation from MonoBehaviour/OnGUI.
    // Long-term goal: Display overlay on agent click (not every frame).
    // Options: Move overlay display to uGUI (Canvas), EditorWindow, or external tool.
    // For now, keep using OnGUI for prototyping; refactor when interactive debug is needed.

    private BtContext _context;
    private StatusEffectManager _statusEffectManager;

    public void Initialize(BtContext context)
    {
        _context = context;
        _statusEffectManager = context.Blackboard.StatusEffectManager;
    }
    
    void OnGUI()
    {
        if (_context == null || _statusEffectManager == null) return;

        var sb = new StringBuilder();
        sb.AppendLine($"<b>{gameObject.name}</b>");
        sb.AppendLine("=== Status Effects ===\n");

        // List all relevant domains you care about
        var allDomains = BlockedDomain.AllDomains;
        
        foreach (var domain in allDomains)
        {
            var isBlocked = _statusEffectManager.IsBlocked(domain);
            sb.AppendLine($"[{domain}]" + (isBlocked ? "  <color=red>BLOCKED</color>" : "  OK"));

            // Show ALL effects impacting this domain
            foreach (var fx in _statusEffectManager.GetActiveEffects())
            {
                if (fx.AffectsDomain(domain))
                {
                    var fxName = fx.Name;
                    var mult = fx.Multipliers.Stats.Movement; // Adapt to per-domain if you support it!
                    var remaining = fx.RemainingDuration;
                    sb.AppendLine($"  - {fxName} (x{mult:0.00}) [{remaining:0.0}s]");
                }
            }
            sb.AppendLine();
        }

        // After the status overlay, add:
        var btRoot = _context?.Controller?.RootNode;
        var activePathSet = new HashSet<IBehaviorNode>();
        BtDebugTools.BuildActivePaths(btRoot, activePathSet);

        sb.AppendLine("\n=== Behavior Tree (Active Paths) ===");
        BtDebugTools.DumpTreeActivePaths(btRoot, sb, 0, activePathSet);
        
        // // After the status overlay, add:
        // var btRoot = _context?.Controller?.RootNode; // or however you access the root node
        // if (btRoot != null)
        // {
        //     sb.AppendLine("\n=== Behavior Tree ===");
        //     BtDebugTools.DumpTree(btRoot, sb);
        // }
        
        // Display the overlay in the game
        GUI.Label(new Rect(10, 10, 350, 700), sb.ToString());
    }

    
}
