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
        private int _lastIndex = -1;
        private int _stableCount = 0;
        private const int MinStableFrames = 6; // adjust as needed
        
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
                if (!(probability > maxProbability)) continue;
                maxIndex = i;
                maxProbability = probability;
            }
            if (maxIndex == _lastIndex)
            {
                _stableCount++;
            }
            else
            {
                _stableCount = 0;
                _lastIndex = maxIndex;
            }
            // Only allow switch if stable for N frames
            return _stableCount >= MinStableFrames ? _lastIndex : (_lastIndex == -1 ? 0 : _lastIndex);
        }
    }
}