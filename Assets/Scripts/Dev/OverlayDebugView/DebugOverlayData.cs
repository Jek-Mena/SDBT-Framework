using System.Collections.Generic;
using System.Text;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
using AI.BehaviorTree.Stimulus;
using Dev.OverlayDebugView.Data;
using Systems.FearPerception;
using Systems.StatusEffectSystem.Component;

namespace Dev.OverlayDebugView
{
    /// <summary>
    /// Pure data/logic class for generating debug overlay information for a single agent.
    /// No Unity dependencies or rendering.
    /// </summary>
    public class DebugOverlayData
    {
        private BtContext _context;
        private StatusEffectManager _statusEffectManager;

        // (Optional) Used for the agent display name
        public string AgentName { get; set; }

        public DebugOverlayData(BtContext context)
        {
            _context = context;
            _statusEffectManager = context.Blackboard.StatusEffectManager;
            AgentName = context.Agent.name;            
        }
        
        /// <summary>
        /// Builds the complete overlay string.
        /// </summary>
        public string BuildDebugString()
        {
            if (_context == null || _statusEffectManager == null)
                return "No context or status manager assigned.";

            var sb = new StringBuilder();
            sb.AppendLine($"Overlay attached to context: {_context}, rootNode: {_context?.Controller?.RootNode}");
            sb.AppendLine($"Current active tree: {_context?.Controller?.ActivePersonaTreeKey}");
            sb.AppendLine($"<b>{AgentName}</b>");
            sb.AppendLine("=== Status Effects ===\n");

            var allDomains = BlockedDomain.AllDomains;

            foreach (var domain in allDomains)
            {
                var isBlocked = _statusEffectManager.IsBlocked(domain);
                sb.AppendLine($"[{domain}]" + (isBlocked ? "  <color=red>BLOCKED</color>" : "  OK"));
                foreach (var fx in _statusEffectManager.GetActiveEffects())
                {
                    if (!fx.AffectsDomain(domain)) continue;
                    var fxName = fx.Name;
                    var mult = fx.Multipliers.Stats.Movement; // Adjust if you want per-domain
                    var remaining = fx.RemainingDuration;
                    sb.AppendLine($"  - {fxName} (x{mult:0.00}) [{remaining:0.0}s]");
                }
                //sb.AppendLine();
            }

            // Behavior Tree (active path)
            var btRoot = _context?.Controller?.RootNode;
            var activePathSet = new HashSet<IBehaviorNode>();
            BtDebugTools.BuildActivePaths(btRoot, activePathSet);

            sb.AppendLine("\n=== Behavior Tree (Active Paths) ===");
            BtDebugTools.DumpTreeActivePaths(btRoot, sb, 0, activePathSet);

            return sb.ToString();
        }

        public IEnumerable<StimulusDebugInfo> GetStimuliForGraph()
        {
            var stimuli = _context.Blackboard.Get<List<FearStimulus>>(BlackboardKeys.Fear.StimuliNearby);
            if (stimuli == null) yield break;

            foreach (var stim in stimuli)
            {
                yield return new StimulusDebugInfo
                {
                    Type = stim.Name,
                    Intensity = stim.Strength,
                    Radius = stim.Radius,
                    TimeRemaining = stim.EffectDuration,
                    Position = stim.Position
                };
            }
        }
        
        public float GetCurrentStimulusValue()
        {
            return _context.Blackboard.Get<float>(BlackboardKeys.Fear.StimulusLevel);
        }
        
        public List<CurveProfileEntry> GetCurveProfiles()
        {
            return _context.AgentProfiles.GetCurveProfile(BtAgentJsonFields.AgentProfiles.DefaultCurves);
        }
    }
}
