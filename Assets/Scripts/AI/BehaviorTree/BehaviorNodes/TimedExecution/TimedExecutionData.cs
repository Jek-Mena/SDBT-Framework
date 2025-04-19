public class TimedExecutionData
{
    public string key = "";
    public float duration = 1f;           // Core timer value for how long the behavior runs
    public float startDelay = 0f;         // Optional: Delay before the execution starts
    public bool interruptible = true;     // Can be interrupted by external events like stun
    public bool failOnInterrupt = true;   // Whether BT returns Failure if interrupted
    public bool resetOnExit = true;       // If false, retains progress when node re-enters

    public TimerExecutionMode mode = TimerExecutionMode.Normal; // Optional: ticking mode
}