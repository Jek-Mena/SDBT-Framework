namespace AI.BehaviorTree.Nodes.TemporalControl.Data
{
    public class TimedExecutionData
    {
        public string Label = "";
        public string TimerId = "";
        public float Duration = 1f;           // Core timer value for how long the behavior runs
        public float StartDelay = 0f;         // Optional: Delay before the execution starts
        public bool Interruptible = true;     // Can be interrupted by external events like stun
        public bool FailOnInterrupt = true;   // Whether BT returns Failure if interrupted
        public bool ResetOnExit = true;       // If false, retains progress when node re-enters

        public TimeExecutionMode Mode = TimeExecutionMode.Normal; // Optional: ticking mode
    }
}