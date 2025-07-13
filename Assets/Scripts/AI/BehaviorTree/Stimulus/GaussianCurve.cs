using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Nodes.Abstractions;
using UnityEngine;

namespace AI.BehaviorTree.Stimulus
{
    public class GaussianCurve : IStimulusCurve
    {
        private readonly string _curveName;
        private readonly float _center, _sharpness, _max;

        public GaussianCurve(string curveName, float center, float sharpness, float max = 1f)
        {
            _curveName = curveName;
            _center = center;
            _sharpness = sharpness;
            _max = max;
        }

        public float Evaluate(float stimulus)
        {
            return _max * Mathf.Exp(-_sharpness * Mathf.Pow(stimulus - _center, 2f));
        }
    }
}