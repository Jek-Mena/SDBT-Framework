// The variable name here should be the same in the JSON file

namespace AI.BehaviorTree.Nodes.Actions.Movement.Data
{
    public class MovementData
    {
        public MovementNodeType MovementType;
        public float Speed;
        public float AngularSpeed;
        public float Acceleration;
        public float StoppingDistance;
        public float UpdateThreshold = 0f; // default to "always update"
        public Direction Direction = Direction.Forward; // Optional for certain Movement (e.g. Transform)
    }
}