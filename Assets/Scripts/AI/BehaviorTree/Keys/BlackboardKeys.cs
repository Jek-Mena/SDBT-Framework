public static class BlackboardKeys
{
    public static class Skill
    {
        public static string Targeting(string skillKey) => $"Skill.Targeting.{skillKey}";
        public static string Target(string skillKey) => $"Skill.Target.{skillKey}";
        public static string Cooldown(string skillKey) => $"Skill.Cooldown.{skillKey}";
        public static string Timing(string skillKey) => $"Skill.Timing.{skillKey}";
        public static string Activation(string skillKey) => $"Skill.Activation.{skillKey}";
        public static string Movement(string skillKey) => $"Skill.Movement.{skillKey}";
        // Add more as needed for your use cases.
    }    
}