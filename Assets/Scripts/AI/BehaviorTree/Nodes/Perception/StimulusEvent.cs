namespace AI.BehaviorTree.Nodes.Perception
{
    public struct StimulusEvent
    {
        public int   sourceId;     // RuntimeIdRegistry id of emitter
        public int   targetId;     // agent id that will consume (usually "self")
        public StimulusType type;  // Vision, Audio, Damage, Proximity, etc.
        public float intensity;    // normalized 0..1 (or domain-specific)
        public Unity.Mathematics.float3 position;
        public double time;        // Time.timeAsDouble snapshot
        public int    aux0;        // optional (e.g., team, tag index)
        public float  aux1;        // optional (e.g., distance)
    }
}