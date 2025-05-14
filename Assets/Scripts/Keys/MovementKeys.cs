public static class MovementKeys
{
    public static class Plugin
    {
        public const string NavMeshMoveToTarget = "Plugin/NavMeshMoveToTarget";
        public const string RigidBodyImpulse = "Plugin/RigidbodyImpulseMover";
        
        public static class Modifiers
        {
            public const string Movement = "Plugin/MovementModifier";
            public const string Impulse = "Plugin/ImpulseModifier";
        }
    }

    public static class Schema
    {
        public const string NavMeshMoveToTarget = "Schema/NavMeshMoveToTarget";
        public const string Impulse = "Schema/RigidbodyImpulseMover";
    }

    public static class Alias
    {
        public const string MoveTo = "Bt/MoveTo";
        public const string ImpulseMover = "Bt/ImpulseMover";
    }
    public static class Json
    {
        public const string Speed = "speed";
        public const string AngularSpeed = "angularSpeed";
        public const string Acceleration = "acceleration";
        public const string StoppingDistance = "stoppingDistance";
        public const string UpdateThreshold = "updateThreshold";
        public const string Target = "target";

        // Impulse-only
        public const string ImpulseStrength = "impulseStrength";
        public const string Tolerance = "tolerance";
        public const string StateTimeout = "stateTimeout";
    }

    public static class ExecutionPhase
    {
        public static readonly PluginExecutionPhase Default = PluginExecutionPhase.Context;
    }

    public static class Domain
    {
        public const string Default = "Domain/Movement";
    }
}