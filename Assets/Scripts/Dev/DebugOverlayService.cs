using System.Text;
using AI.BehaviorTree.Runtime.Context;

namespace Dev
{
    /// <summary>
    /// Pure logic for building behavior tree/agent debug info as a string or data object.
    /// </summary>
    public class DebugOverlayService
    {
        /// <summary>
        /// Returns a human-readable debug string for the agent’s BT context.
        /// </summary>
        public string GetDebugString(BtContext context)
        {
            if (context == null) return "No context selected.";

            var sb = new StringBuilder();
            sb.AppendLine($"Agent: {context.Agent.name} (Session: {context.Blackboard.BtSessionId})");
            sb.AppendLine("State: {context.AgentState}");
            sb.AppendLine("Behavior Tree: {context.ActiveBehaviorTreeName}");
            sb.AppendLine();

            return "";
        }
    }
}