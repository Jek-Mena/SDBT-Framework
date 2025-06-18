using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ProfileContextBuilder : IContextBuilderModule
{
    public void Build(BtContext context)
    {
        var scriptName = nameof(ProfileContextBuilder);
        var blackboard = context.Blackboard;
        var config = blackboard.Get<ConfigData>(PluginMetaKeys.Core.BtConfig.Plugin);
        if (config == null)
            throw new Exception($"[{scriptName}] BtConfig missing!");
        
        Debug.Log("[{scriptName}] Parsing profiles...");
        // -- Parse each profile block --
        blackboard.TargetingProfiles = ParseProfileBlock<TargetingData>(config.RawJson, CoreKeys.ProfilesBlock.Targeting);
        blackboard.MovementProfiles  = ParseProfileBlock<MovementData> (config.RawJson, CoreKeys.ProfilesBlock.Movement);
        blackboard.RotationProfiles = ParseProfileBlock<RotationData>(config.RawJson, CoreKeys.ProfilesBlock.Rotation);
        blackboard.TimingProfiles = ParseProfileBlock<TimedExecutionData>(config.RawJson, CoreKeys.ProfilesBlock.Timing);
        
        // Add similar.... 

    }

    private Dictionary<string, TProfile> ParseProfileBlock<TProfile>(JObject root, string blockKey)
    {
        var block = root[blockKey] as JObject;
        if (block == null)
        {
            // Try nested under "profiles"
            var profiles = root[CoreKeys.Profiles] as JObject;
            if (profiles != null)
                block = profiles[blockKey] as JObject;
        }
        if (block == null)
            return new Dictionary<string, TProfile>();

        var profilesDict = new Dictionary<string, TProfile>();
        foreach (var prop in block.Properties())
        {
            profilesDict[prop.Name] = prop.Value.ToObject<TProfile>();
        }
        return profilesDict;
    }
}