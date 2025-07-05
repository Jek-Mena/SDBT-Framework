using System;
using System.Collections.Generic;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using AI.BehaviorTree.Switching;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ProfileBuilderModule : IContextBuilderModule
{
    private const string ScriptName = nameof(ProfileBuilderModule);
    private readonly JObject _agentConfig;
    
    public void Build(BtContext context)
    {
        var blackboard = context.Blackboard;
        
        // Injects the full config JObject into the blackboard at context build time.
        var runtimeData = context.Agent.GetComponent<EntityRuntimeData>();
        blackboard.Set(BlackboardKeys.EntityConfig, runtimeData);
        
        // Load both profile blocks (agent and behavior)
        var agentProfiles = runtimeData.Definition.Config[CoreKeys.AgentProfiles] as JObject;
        var behaviorProfiles = runtimeData.Definition.Config[CoreKeys.BehaviorProfiles] as JObject;
        
        // AGENT-GLOBAL PROFILES
        // Only used for systems like Fear, Health, etc.
        if (agentProfiles != null)
        {
            Debug.Log($"[{ScriptName}] Parsing agent-global profiles...");
            blackboard.HealthProfiles = ParseProfileBlock<HealthData>(agentProfiles, AgentProfileSelectorKeys.Health.Profiles);
            blackboard.FearProfiles = ParseProfileBlock<FearPerceptionData>(agentProfiles, AgentProfileSelectorKeys.Fear.Profiles);
            blackboard.SwitchProfiles = ParseProfileBlockList<SwitchCondition>(agentProfiles, AgentProfileSelectorKeys.Switch.Profiles);
            Debug.Log($"[{ScriptName}] Finished parsing agent-global profiles...");
        }
        else
        {
            Debug.LogError($"[{ScriptName}] {CoreKeys.AgentProfiles} block is missing!");
        }
        
        // BEHAVIOR PROFILES
        // Only used for BT node config: movement, timing, targeting, etc.
        if (behaviorProfiles != null)
        {
            Debug.Log($"[{ScriptName}] Parsing behavior profiles...");
            blackboard.TargetingProfiles = ParseProfileBlock<TargetingData>(behaviorProfiles, BtNodeProfileSelectorKeys.Targeting);
            blackboard.MovementProfiles  = ParseProfileBlock<MovementData> (behaviorProfiles, BtNodeProfileSelectorKeys.Movement);
            blackboard.RotationProfiles = ParseProfileBlock<RotationData>(behaviorProfiles, BtNodeProfileSelectorKeys.Rotation);
            blackboard.TimingProfiles = ParseProfileBlock<TimedExecutionData>(behaviorProfiles, BtNodeProfileSelectorKeys.Timing);
            Debug.Log($"[{ScriptName}] Finished parsing behavior profiles...");
        }
        else
        {
            Debug.LogError($"[{ScriptName}] {CoreKeys.BehaviorProfiles} block is missing!");       
        }
    }

    /// <summary>
    /// Attempts to parse a dictionary of profiles (of type TProfile) from the provided JSON object.
    /// - Handles both flat (root-level) and nested ("profiles" block) JSON structures.
    /// - Returns an empty dictionary if the block is not found.
    /// </summary>
    private Dictionary<string, TProfile> ParseProfileBlock<TProfile>(JObject root, string blockKey)
    {
        // Try to find the block at the root level
        var block = root[blockKey] as JObject;
        
        if (block == null)
        {
            Debug.LogError($"[{ScriptName}] Profile block '{blockKey}' missing in profile block.");
            return new Dictionary<string, TProfile>();
        }

        var profilesDict = new Dictionary<string, TProfile>();
        foreach (var prop in block.Properties())
        {
            try
            {
                profilesDict[prop.Name] = prop.Value.ToObject<TProfile>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{ScriptName}] Failed to parse '{blockKey}' profile '{prop.Name}': {ex}");
            }
        }
        return profilesDict;
    }
    
    private Dictionary<string, List<TElement>> ParseProfileBlockList<TElement>(JObject root, string blockKey)
    {
        // Try to find the block at the root level
        var block = root[blockKey] as JObject;
        
        if (block == null)
        {
            Debug.LogError($"[{ScriptName}] Profile block '{blockKey}' missing in profile block.");
            return new Dictionary<string, List<TElement>>();
        }

        var profilesDict = new Dictionary<string, List<TElement>>();
        foreach (var prop in block.Properties())
        {
            try
            {
                // prop.Value is expected to be a JArray
                profilesDict[prop.Name] = prop.Value.ToObject<List<TElement>>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{ScriptName}] Failed to parse '{blockKey}' profile '{prop.Name}': {ex}");
            }
        }
        return profilesDict;
    }
}