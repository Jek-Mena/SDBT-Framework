using AI.BehaviorTree.Runtime.Context;
using Dev;
using Utils.Component;

namespace AI.BehaviorTree.Registry.ContextBuilderModules
{
    public class DebugOverlayBuilderModule : IContextBuilderModule
    {
        public void Build(BtContext context)
        {
            var scriptName = nameof(DebugOverlayBuilderModule);
            var agent = context.Agent;
            var blackboard = context.Blackboard;
        
            // Injects the full config JObject into the blackboard at context build time.
            var debugOverlay = agent.RequireComponent<DebugOverlay>();
        
            debugOverlay.Initialize(context);
        }
    }
}