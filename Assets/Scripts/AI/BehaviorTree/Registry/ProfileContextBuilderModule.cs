using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ProfileContextBuilderModule : IContextBuilderModule
{
    private readonly JObject _agentConfig;
    
    public void Build(BtContext context)
    {
        var scriptName = nameof(ProfileContextBuilderModule);
        var blackboard = context.Blackboard;
        
        // Injects the full config JObject into the blackboard at context build time.
        var runtimeData = context.Agent.GetComponent<EntityRuntimeData>();
        blackboard.Set(BlackboardKeys.EntityConfig, runtimeData);
        
        var profiles = runtimeData.Definition.Config[CoreKeys.Profiles] as JObject;
        if (profiles == null)
            throw new Exception($"[{scriptName}] BtConfig missing!");
        
        // -- Parse each profile block --
        Debug.Log("[{scriptName}] Parsing profiles...");
        blackboard.SwitchProfiles = ParseProfileBlockList<SwitchCondition>(profiles, CoreKeys.ProfilesBlock.Switches);
        blackboard.HealthProfiles = ParseProfileBlock<HealthData>(profiles, CoreKeys.ProfilesBlock.Health);
        blackboard.TargetingProfiles = ParseProfileBlock<TargetingData>(profiles, CoreKeys.ProfilesBlock.Targeting);
        blackboard.MovementProfiles  = ParseProfileBlock<MovementData> (profiles, CoreKeys.ProfilesBlock.Movement);
        blackboard.RotationProfiles = ParseProfileBlock<RotationData>(profiles, CoreKeys.ProfilesBlock.Rotation);
        blackboard.TimingProfiles = ParseProfileBlock<TimedExecutionData>(profiles, CoreKeys.ProfilesBlock.Timing);
        
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
            return new Dictionary<string, TProfile>();

        var profilesDict = new Dictionary<string, TProfile>();
        foreach (var prop in block.Properties())
        {
            try
            {
                profilesDict[prop.Name] = prop.Value.ToObject<TProfile>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ParseProfileBlock] Failed to parse '{blockKey}' profile '{prop.Name}': {ex}");
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
            return new Dictionary<string, List<TElement>>();

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
                Debug.LogError($"[ParseProfileBlockList] Failed to parse '{blockKey}' profile '{prop.Name}': {ex}");
            }
        }
        return profilesDict;
    }
}