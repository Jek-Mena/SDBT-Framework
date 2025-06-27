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
        debugOverlay.SetStatusEffectManager(blackboard.StatusEffectManager);
    }
}

public class BehaviorIntentExecutorBuilderModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(BehaviorIntentExecutorBuilderModule);
        var agent = context.Agent;
        var blackboard = context.Blackboard;
        
        // Injects the full config JObject into the blackboard at context build time.
        var debugOverlay = agent.RequireComponent<FleeIntentExecutor>();
        
        debugOverlay.Initialize(context);
        debugOverlay.SetStatusEffectManager(blackboard.StatusEffectManager);
    }
}