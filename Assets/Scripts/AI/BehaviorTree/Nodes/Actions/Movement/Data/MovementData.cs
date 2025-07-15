// The variable name here should be the same in the JSON file

namespace AI.BehaviorTree.Nodes.Actions.Movement.Data
{
    public class MovementData
    {
        public MoveToTargetNodeType MovementType;
        public float Speed;
        public float AngularSpeed;
        public float Acceleration;
        public float StoppingDistance;
        public float UpdateThreshold = 0f; // default to "always update"
        public Direction Direction = Direction.Forward; // Optional for certain Movement (e.g. Transform)
    }
}

// TODO [SYSTEMIC DATA VALIDATION] [2025-07-15]
// ------------------------------------------------------------------------------------------
// Any time you add an enum/config-driven field (e.g., MovementType, RotationType, PerceptionType)
// that is deserialized from JSON/config, ALWAYS:
//
// 1. Validate after load/deserialization:
//      - If the enum value is 'None', 'Default', or invalid, throw or log an error IMMEDIATELY.
//      - Do not let 'None' values propagate to routers/dispatchers—fail early, not during BT ticks.
//
// 2. Document expected JSON strings in XML docs above the field and/or enum.
//
// 3. (Optional) Write an editor or startup validation utility that scans all configs
//    and prints a warning for any missing/invalid enum value.
//
// 4. Log actionable error messages with the invalid value and valid options.
//
// This prevents silent config/enum mismatches and makes bugs obvious at the source.
// ------------------------------------------------------------------------------------------
//
// Example usage (MovementData, RotationData, etc):
//
// public void Validate()
// {
//     if (MovementType == MoveToTargetNodeType.None)
//         throw new InvalidOperationException(
//             "[MovementData] MovementType is None after deserialization. " +
//             "Check your BT JSON/config for missing or invalid MovementType string."
//         );
// }
