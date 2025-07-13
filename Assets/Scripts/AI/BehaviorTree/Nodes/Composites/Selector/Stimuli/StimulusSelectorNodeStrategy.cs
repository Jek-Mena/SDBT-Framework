using System.Collections.Generic;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Runtime.Context;
using UnityEngine;

namespace AI.BehaviorTree.Nodes.Composites.Selector.Stimuli
{
    public class StimulusSelectorNodeStrategy : IChildSelectorStrategy
    {
        private readonly List<IStimulusCurve> _stimulusCurves;
        private readonly string _stimulusKey;

        public StimulusSelectorNodeStrategy(string stimulusKey, List<IStimulusCurve> stimulusCurves)
        {
            _stimulusCurves = stimulusCurves;
            _stimulusKey = stimulusKey;
        }

        public int SelectChildIndex(IReadOnlyList<IBehaviorNode> children, BtContext context)
        {
            var stimulus = context.Blackboard.Get<float>(_stimulusKey);
            var maxIndex = 0;
            var maxProbability = float.MinValue;

            for (var i = 0; i < _stimulusCurves.Count && i < children.Count; i++)
            {
                var probability = _stimulusCurves[i].Evaluate(stimulus);
                Debug.Log($"[StimulusSelector] Curve: {_stimulusCurves[i]}, Value: {probability:F2}, Stimulus: {stimulus:F2}");
                if (!(probability > maxProbability)) continue;
                
                maxIndex = i;
                maxProbability = probability;
            }
            Debug.Log($"[StimulusSelector] PICKED INDEX: {maxIndex} (curve: {_stimulusCurves[maxIndex]})");
            return maxIndex;
        }
    }
}