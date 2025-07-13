namespace AI.BehaviorTree.Stimulus
{
    public class CurveProfileEntry
    {
        public string CurveName { get; set; }
        public string CurveType { get; set; } // "Sigmoid", "Gaussian", etc.
        public float Center { get; set; }
        public float Sharpness { get; set; }
        public float Max { get; set; }
        public string StimuliBehaviorTree { get; set; } // Subtree reference/key
    }
}