using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Abstractions;
using UnityEngine;

namespace AI.BehaviorTree.Stimulus
{
    public class SigmoidCurve : IStimulusCurve
    {
        private readonly string _curveName;
        private readonly float _center, _sharpness, _max;
        
        public SigmoidCurve(string curveName, float center, float sharpness, float max = 1f)
        {
            _curveName = curveName;
            _center = center;
            _sharpness = sharpness;
            _max = max;
        }
        
        public float Evaluate(float stimulus)
        {
            return _max / (1f + Mathf.Exp(-_sharpness * (stimulus - _center)));
        }
    }
}