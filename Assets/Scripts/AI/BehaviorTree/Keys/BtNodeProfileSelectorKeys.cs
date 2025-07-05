namespace AI.BehaviorTree.Keys
{
    /// <summary>
    /// Defines all SINGULAR config keys used to select a profile for a behavior tree node.
    /// - These keys always live in the "config" block of a node in a BT definition.
    /// - They refer to keys inside "behaviorProfiles" (never agentProfiles!).
    ///
    /// Example: { "type": "Bt/MoveToTarget", "config": { "movementProfile": "ChaserMovement" } }
    /// </summary>
    public static class BtNodeProfileSelectorKeys
    {
        public const string Movement = "MovementProfiles";
        public const string Targeting = "TargetingProfiles";
        public const string Rotation = "RotationProfiles";
        public const string Timing = "TimingProfiles";
        // Add more as you introduce new per-node systems.       
    }
}