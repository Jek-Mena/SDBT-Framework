using System;
using System.Collections.Generic;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Actions.Movement.Data;
using AI.BehaviorTree.Nodes.Actions.Rotate.Data;
using AI.BehaviorTree.Nodes.TemporalControl.Data;
using AI.BehaviorTree.Runtime.Context;
using AI.BehaviorTree.Stimulus;
using AI.BehaviorTree.Switching;
using AI.GroupAI;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AI.BehaviorTree.Registry.ContextBuilderModules
{
    public class ProfileBuilderModule : IContextBuilderModule
    {
        private const string ScriptName = nameof(ProfileBuilderModule);
        private readonly JObject _agentConfig;
    
        public void Build(BtContext context)
        {
            var agentProfiles = context.AgentProfiles;
            
            // Agents Current Profiles
            if (context.AgentDefinition.Config == null)
                Debug.LogError($"[{ScriptName}] Agent config missing!");
            
            // Load both profile blocks (agent and behavior)
            var rawAgentProfiles = context.AgentDefinition.Config[BtAgentJsonFields.AgentProfilesField] as JObject;
            var rawBehaviorProfiles = context.AgentDefinition.Config[BtAgentJsonFields.BehaviorProfilesField] as JObject;
    
            // AGENT-GLOBAL PROFILES
            // Only used for systems like Fear, Health, etc.
            if (rawAgentProfiles != null)
            {
                Debug.Log($"[{ScriptName}] Parsing agent-global profiles...");
                // --- Health ---
                agentProfiles.HealthProfiles = ParseProfileBlock<HealthData>
                    (rawAgentProfiles, BtAgentJsonFields.AgentProfiles.HealthProfiles);
                // --- Fear
                agentProfiles.FearProfiles = ParseProfileBlock<FearPerceptionData>
                    (rawAgentProfiles, BtAgentJsonFields.AgentProfiles.FearProfiles);
                // --- Curve
                agentProfiles.CurveProfiles = ParseProfileBlockList<CurveProfileEntry>
                    (rawAgentProfiles, BtAgentJsonFields.AgentProfiles.CurveProfiles);
                // --- Persona
                agentProfiles.PersonaProfiles = ParseProfileBlockList<PersonaSwitchRule>
                    (rawAgentProfiles, BtAgentJsonFields.AgentProfiles.PersonaProfiles);
                // --- Group
                agentProfiles.GroupFormationProfiles = ParseProfileBlockList<FormationProfileEntry>
                    (rawAgentProfiles, BtAgentJsonFields.AgentProfiles.GroupFormationProfiles);

                // Debug.Log($"[{ScriptName}]🟣HealthProfiles:   {agentProfiles.HealthProfiles.Count}");
                // Debug.Log($"[{ScriptName}]🟣FearProfiles:     {agentProfiles.FearProfiles.Count}");
                // Debug.Log($"[{ScriptName}]🟣CurveProfiles:    {agentProfiles.CurveProfiles.Count}");
                // Debug.Log($"[{ScriptName}]🟣PersonaProfiles:  {agentProfiles.PersonaProfiles.Count}");
                // Debug.Log($"[{ScriptName}]Finished parsing agent-global profiles...");
            }
            else
            {
                Debug.LogError($"[{ScriptName}] {BtAgentJsonFields.AgentProfilesField} block is missing!");
            }
        
            // Agents Current Profiles
            agentProfiles.CurrentPersonaProfile =
                context.AgentDefinition.Config[BtAgentJsonFields.CurrentPersonaProfile]?.ToString();
            agentProfiles.CurrentFormationProfile =
                context.AgentDefinition.Config[BtAgentJsonFields.CurrentFormationProfile]?.ToString();
            // Debug.Log($"🟡 [{ScriptName}] Set CurrentPersonaProfileKey = '{personaProfileKey}'");
            // Debug.Log($"🟡 [{ScriptName}] PersonaProfiles loaded. Keys: [{string.Join(", ", agentProfiles.PersonaProfiles.Keys)}]");
            // Add Health and Fear
        
            // BEHAVIOR PROFILES
            // Only used for BT node config: movement, timing, targeting, etc.
            if (rawBehaviorProfiles != null)
            {
                //Debug.Log($"[{ScriptName}] Parsing behavior profiles...");
                agentProfiles.TargetingProfiles = ParseProfileBlock<TargetingData>
                    (rawBehaviorProfiles,BtAgentJsonFields.BehaviorProfiles.TargetingProfiles);
                agentProfiles.MovementProfiles  = ParseProfileBlock<MovementData>
                    (rawBehaviorProfiles,BtAgentJsonFields.BehaviorProfiles.MovementProfiles);
                agentProfiles.RotationProfiles = ParseProfileBlock<RotationData>
                    (rawBehaviorProfiles,BtAgentJsonFields.BehaviorProfiles.RotationProfiles);
                agentProfiles.TimingProfiles = ParseProfileBlock<TimedExecutionData>
                    (rawBehaviorProfiles,BtAgentJsonFields.BehaviorProfiles.TimingProfiles);
                //Debug.Log($"[{ScriptName}] Finished parsing behavior profiles...");
            }
            else
            {
                Debug.LogError($"[{ScriptName}] {BtAgentJsonFields.BehaviorProfilesField} block is missing!");       
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
}