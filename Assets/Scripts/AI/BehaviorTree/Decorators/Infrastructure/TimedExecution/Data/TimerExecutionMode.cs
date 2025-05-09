public enum TimerExecutionMode
{
    Normal,       // Just runs once for the duration
    Loop,         // Repeats after duration expires
    UntilSuccess, // Continues looping until child/condition is met
    UntilFailure  // Continues looping until failure
}