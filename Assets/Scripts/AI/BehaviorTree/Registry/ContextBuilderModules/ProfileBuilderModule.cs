using System;
using System.Collections.Generic;
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
        
        var profiles = runtimeData.Definition.Config[CoreKeys.Profiles] as JObject;
        if (profiles == null)
            throw new Exception($"[{ScriptName}] BtConfig missing!");
        
        // -- Parse each profile block --
        Debug.Log("[{scriptName}] Parsing profiles...");
        blackboard.SwitchProfiles = ParseProfileBlockList<SwitchCondition>(profiles, AgentConfigProfileBlocks.Switches);
        blackboard.HealthProfiles = ParseProfileBlock<HealthData>(profiles, AgentConfigProfileBlocks.Health);
        blackboard.TargetingProfiles = ParseProfileBlock<TargetingData>(profiles, AgentConfigProfileBlocks.Targeting);
        blackboard.MovementProfiles  = ParseProfileBlock<MovementData> (profiles, AgentConfigProfileBlocks.Movement);
        blackboard.RotationProfiles = ParseProfileBlock<RotationData>(profiles, AgentConfigProfileBlocks.Rotation);
        blackboard.TimingProfiles = ParseProfileBlock<TimedExecutionData>(profiles, AgentConfigProfileBlocks.Timing);
        blackboard.FearProfiles = ParseProfileBlock<FearPerceptionData>(profiles, AgentConfigProfileBlocks.FearPerception);
        // Add similar.... 
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
        
        // If not found, try under the standard "profiles" section (using your convention)
        if (block == null)
        {
            var profilesSection = root[CoreKeys.Profiles] as JObject;
            if (profilesSection != null)
                block = profilesSection[blockKey] as JObject;
        }
        
        // If still not found, return an empty dictionary
        if (block == null)
        {
            Debug.LogWarning($"[{ScriptName}] Profile block '{blockKey}' missing. If this is required for agent function, update your JSON config.");
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
        var block = root[blockKey] as JObject;

        if (block == null)
        {
            var profilesSection = root[CoreKeys.Profiles] as JObject;
            if (profilesSection != null)
                block = profilesSection[blockKey] as JObject;
        }

        if (block == null)
        {
            Debug.LogWarning($"[{ScriptName}] Profile block '{blockKey}' missing. If this is required for agent function, update your JSON config.");
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