using System.Collections.Generic;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Registry.ContextBuilderModules.Abstraction;
using AI.BehaviorTree.Runtime.Context;
using Systems.FearPerception;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Perception.Fear
{
    /// <summary>
    /// Always-sensing fear perception. Queries the manager, processes fear, and writes to the blackboard.
    /// </summary>
    public class FearPerception : PerceptionModule<FearStimulus, FearPerceptionData>
    {
        private const string ScriptName = nameof(FearPerception);
    
        public override void Initialize(BtContext context)
        {
            base.Initialize(context);
            
            Profile = context.AgentProfiles.GetFearPerceptionProfile(BtAgentJsonFields.AgentProfiles.DefaultFear);
            if (Profile == null)
                Debug.LogError($"[{ScriptName}] No FearPerceptionData profile found for this agent/context!");
            else
                Debug.Log($"[{ScriptName}] Profile type: {Profile.GetType()}\nProfile JSON: {JsonUtility.ToJson(Profile)}");
        }
        
        protected override void ProcessStimuli(List<FearStimulus> stimuli)
        {
            var position = transform.position;
            var totalFear = 0f;
            var maxContribution = 0f;
            FearStimulus? mainThreat = null;

            if (stimuli != null && stimuli.Count != 0)
            {
                // Debug.Log($"[{ScriptName}] Agent '{name}' received {stimuli.Count} fear stimuli. My position: {position}");

                foreach (var stim in stimuli)
                {
                    var distance = Vector3.Distance(position, stim.Position);
                    //Debug.Log($"[{ScriptName}] - Stimulus at {stim.Position}, radius: {stim.Radius}, strength: {stim.Strength}, distance: {distance}");

                    if (distance < stim.Radius)
                    {
                        // Simple weighted: linear falloff
                        var contribution = stim.Strength * (1f - distance / stim.Radius);
                        totalFear += contribution;
                        // Debug.Log($"[{ScriptName}] -- INSIDE range! Contribution: {contribution}");

                        if (!(contribution > maxContribution)) continue;
                        
                        maxContribution = contribution;
                        mainThreat = stim;
                    }
                    else
                    {
                        //Debug.Log($"[{ScriptName}] -- OUTSIDE range.");
                    }
                }
            }
            else
            {
                //Debug.Log($"[{ScriptName}] Agent '{name}' detected NO fear stimuli this tick.");
            }

            // Normalize TotalFear
            // Pick a "maxExpectedFear" value that makes sense for your game.
            // Example: if a single strong threat can contribute 2, and 2-3 is max "panic", use 2 or 3.
            // Adjust this based on the config!
            var maxExpectedFear = 2.0f; // <-- Adjust as needed
            var normalizedFear = Mathf.Clamp01(totalFear / maxExpectedFear);
            
            //Debug.Log($"[{ScriptName}]🟠Writing FearStimulusLevel={normalizedFear}");
            Context.Blackboard.Set(BlackboardKeys.Fear.StimulusLevel, normalizedFear);

            if (mainThreat.HasValue)
            {
                //Debug.Log($"[{ScriptName}] Main threat: {mainThreat.Value.Position}, Max Contribution: {maxContribution}");
                Context.Blackboard.Set(BlackboardKeys.Fear.Source, mainThreat.Value.Source);
                
                var value = Context.Blackboard.Get<object>(BlackboardKeys.Fear.Source);
                // Debug.Log($"[FearPerception] Set FearSource: Type={value?.GetType().Name}, Value={value}");
                // if (value is GameObject go) Debug.Log($"[FearPerception] FearSource GameObject Name: {go.name}");
                // if (value is Transform tf) Debug.Log($"[FearPerception] FearSource Transform: {tf.position}");
                // if (value is Vector3 v3) Debug.Log($"[FearPerception] FearSource Vector3: {v3}");
                
                
                // Agent runs away from the main threat (source) at a fixed distance.
                var agentPos = Context.Agent.transform.position;
                var threatPos = mainThreat.Value.Position;
                
                // Defensive: If agent and threat are at the same spot, use some fallback
                var fleeDir = (agentPos - threatPos);
                if (fleeDir.sqrMagnitude < 0.01f)
                    fleeDir = Vector3.forward; // fallback direction
                
                fleeDir.Normalize();
                
                var fleeDistance = Profile.FleeDistance;
                var fleePoint = agentPos + fleeDir * fleeDistance;

                Context.Blackboard.Set(BlackboardKeys.Fear.FleePoint, fleePoint);
            }
            else
            {
                //Debug.Log($"[{ScriptName}] No main threat found.");
                Context.Blackboard.Remove(BlackboardKeys.Fear.Source);
                Context.Blackboard.Remove(BlackboardKeys.Fear.FleePoint);
            }
        }
    
        protected override List<FearStimulus> QueryStimuli()
        {
            if (Profile == null)
            {
                Debug.LogError($"[{ScriptName}] Profile missing; cannot query stimuli.");
                return new List<FearStimulus>();
            }

            return FearEmitterManager.Instance?.Query(transform.position, Profile.DetectionRange) 
                   ?? new List<FearStimulus>();
        }
    
        protected override void WriteStimuliToBlackboard(List<FearStimulus> stimuli)
        {
            Context.Blackboard.Set(BlackboardKeys.Fear.StimuliNearby, stimuli);
        }
    }
}