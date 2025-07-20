using System;
using System.Collections.Generic;
using AI.BehaviorTree.Loader;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Registry;
using AI.BehaviorTree.Runtime.Context;
using AI.BehaviorTree.Stimulus;
using Keys;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Composites.Selector.Stimuli
{
    public class BtStimuliSelectorNodeFactory : IBtNodeFactory
    {
        private const string ScriptName = nameof(BtStimuliSelectorNodeFactory);
        
        public IBehaviorNode CreateNode(TreeNodeData nodeData, BtContext context, Func<TreeNodeData, IBehaviorNode> buildChildNodeRecurs)
        {
            var config = nodeData.Settings;
            var stimulusKey = config[BtJsonFields.Config.Stimulus]?.ToString();
            var curveProfileId = config[BtJsonFields.Config.CurveProfile]?.ToString();
            
            var agentProfiles = context.AgentProfiles;
            var profileEntries = agentProfiles.GetCurveProfile(curveProfileId);
            
            var stimuliCurves = new List<IStimulusCurve>();
            var children = new List<IBehaviorNode>();
            
            foreach (var entry in profileEntries)
            {
                Debug.Log($"[{ScriptName}]🟣Curve entry:\n{JsonConvert.SerializeObject(entry, Formatting.Indented)}");
                IStimulusCurve curve = entry.CurveType switch
                {
                    // TODO remove magic strings
                    "Sigmoid" => new SigmoidCurve(entry.CurveName, entry.Center, entry.Sharpness, entry.Max),
                    "Gaussian" => new GaussianCurve(entry.CurveName, entry.Center, entry.Sharpness, entry.Max),
                    _ => throw new Exception($"Unknown curve type: {entry.CurveType}")
                };
                stimuliCurves.Add(curve);
                
                var treeKey = entry.StimuliBehaviorTree; // e.g., "move_and_wait"
                
                var btJson = BtConfigRegistry.RequireRootNode(treeKey);
                
                Debug.Log($"[{ScriptName}]About to build tree '{treeKey}':\n{btJson?.ToString(Formatting.Indented) ?? "NULL"}");

                var treeNodeData = new TreeNodeData(btJson);
                var childNode = buildChildNodeRecurs(treeNodeData);
                children.Add(childNode);
            }
            
            var strategy = new StimulusSelectorNodeStrategy(stimulusKey, stimuliCurves);
            return new BtSelectorNode(children, strategy, nameof(StimulusSelectorNodeStrategy));
        }
    }
}