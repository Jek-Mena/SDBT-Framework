using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

public class HealthBuilderModule : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(HealthBuilderModule);
        var agent = context.Agent;
        var blackboard = context.Blackboard;

        var healthComponent = agent.GetComponent<HealthComponent>();
        if (healthComponent == null)
        {
            Debug.LogError($"[{scriptName}] No {nameof(HealthComponent)} found on '{agent.name}'. " + 
                           $"Ensure your AI prefab has a {nameof(HealthComponent)} attached.");
            throw new System.Exception($"[{scriptName}] Health logic missing!");
        }
        
        /*// Initialize with dummy data just to guarantee pipeline safety (real profile will come from the BT node)
        healthComponent.Initialize(new HealthData());
        
        // Inject StatusEffectManager only if supported
        if (healthComponent is IUsesStatusEffectManager effectUser)
        {
            //if (context.Blackboard.StatusEffectManager != null) 
//                effectUser.SetStatusEffectManager(context.Blackboard.StatusEffectManager);
        }
        
        // Set current health into blackboard (for BT logic)
        blackboard.Set(BlackboardKeys.Health.CurrentHealth, healthComponent.CurrentHealth);*/
    }
}