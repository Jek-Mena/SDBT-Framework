using UnityEngine;

public class TargetingContextBuilerModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var blackboard = context.Blackboard;
        var agent = context.Agent;
    }
}