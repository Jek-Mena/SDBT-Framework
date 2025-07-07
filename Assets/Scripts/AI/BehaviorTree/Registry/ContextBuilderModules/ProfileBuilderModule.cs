using System;
using System.Collections.Generic;
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
        var agentProfiles = context.AgentProfiles;
        
        // Load both profile blocks (agent and behavior)
        var rawAgentProfiles = context.Definition.Config[BtEntityJsonFields.AgentProfilesField] as JObject;
        var rawBehaviorProfiles = context.Definition.Config[BtEntityJsonFields.BehaviorProfilesField] as JObject;
        
        // AGENT-GLOBAL PROFILES
        // Only used for systems like Fear, Health, etc.
        if (rawAgentProfiles != null)
        {
            Debug.Log($"[{ScriptName}] Parsing agent-global profiles...");
            agentProfiles.HealthProfiles = ParseProfileBlock<HealthData>
                (rawAgentProfiles, BtEntityJsonFields.AgentProfiles.HealthKey);
            agentProfiles.FearProfiles = ParseProfileBlock<FearPerceptionData>
                (rawAgentProfiles, BtEntityJsonFields.AgentProfiles.FearKey);
            agentProfiles.SwitchProfiles = ParseProfileBlockList<SwitchCondition>
                (rawAgentProfiles, BtEntityJsonFields.AgentProfiles.SwitchKey);
            Debug.Log($"[{ScriptName}] Finished parsing agent-global profiles...");
        }
        else
        {
            Debug.LogError($"[{ScriptName}] {BtEntityJsonFields.AgentProfilesField} block is missing!");
        }
        
        // BEHAVIOR PROFILES
        // Only used for BT node config: movement, timing, targeting, etc.
        if (rawBehaviorProfiles != null)
        {
            Debug.Log($"[{ScriptName}] Parsing behavior profiles...");
            agentProfiles.TargetingProfiles = ParseProfileBlock<TargetingData>
                (rawBehaviorProfiles,BtEntityJsonFields.BehaviorProfiles.TargetingKey);
            agentProfiles.MovementProfiles  = ParseProfileBlock<MovementData>
                (rawBehaviorProfiles,BtEntityJsonFields.BehaviorProfiles.MovementKey);
            agentProfiles.RotationProfiles = ParseProfileBlock<RotationData>
                (rawBehaviorProfiles,BtEntityJsonFields.BehaviorProfiles.RotationKey);
            agentProfiles.TimingProfiles = ParseProfileBlock<TimedExecutionData>
                (rawBehaviorProfiles,BtEntityJsonFields.BehaviorProfiles.TimingKey);
            Debug.Log($"[{ScriptName}] Finished parsing behavior profiles...");
        }
        else
        {
            Debug.LogError($"[{ScriptName}] {BtEntityJsonFields.BehaviorProfilesField} block is missing!");       
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