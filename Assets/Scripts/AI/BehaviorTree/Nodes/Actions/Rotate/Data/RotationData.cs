namespace AI.BehaviorTree.Nodes.Actions.Rotate.Data
{
    public class RotationData
    {
        /// <summary>
        /// Type of look-at behavior.
        /// </summary>
        public RotateToTargetNodeType RotationType;

        /// <summary>
        /// Rotation speed in degrees per second.
        /// </summary>
        public float Speed = 120f; // Example default

        /// <summary>
        /// Max angle (degrees) considered "facing the target."
        /// </summary>
        public float AngleThreshold = 5f;

        /// <summary>
        /// Minimum angle difference (degrees) to trigger a new rotation update.
        /// Prevents unnecessary rotation updates for tiny angle changes.
        /// </summary>
        public float AngleRotationUpdateThreshold = 5f;

        /// <summary>
        /// Squared distance (units^2) considered "arrived at target."
        /// </summary>
        public float SqrArrivalDistanceThreshold = 0.05f;
    }
}