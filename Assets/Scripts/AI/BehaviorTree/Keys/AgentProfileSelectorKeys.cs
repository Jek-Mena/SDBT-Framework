namespace AI.BehaviorTree.Keys
{
    /// <summary>
    /// Defines keys for agent-global profile blocks and selectors.
    /// 
    /// - Each [BLOCK] key points to a dictionary inside "agentProfiles" in the entity config.
    /// - Each [SELECTOR] key is the singular field in the agent config that chooses the default profile for that domain.
    /// - DEFAULT keys are fallback values (useful for error recovery and new agents).
    ///
    /// Only use these for systems where there is exactly one active profile for the *entire* agent (e.g., fear, health).
    /// Do NOT add keys for per-node BT selectors here.
    /// </summary>
    public static class AgentProfileSelectorKeys
    {
        // TODO, DefaultProfile is for audit!
        public static class Fear
        {
            public const string Profiles = "FearProfiles";                  // BLOCK (dictionary of all fear profiles)    
            public const string CurrentProfile = "agentCurrentFearProfile"; // SELECTOR (which fear profile is active for this agent)
            public const string DefaultProfile = "DefaultFear";             // DEFAULT VALUE
        }

        public static class Switch
        {
            public const string Profiles = "SwitchProfiles";
            public const string CurrentProfile = "agentCurrentSwitchProfile";
            public const string DefaultProfile = "DefaultSwitch";   
        }
    }
}