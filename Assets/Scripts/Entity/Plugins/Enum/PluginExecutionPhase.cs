/// <summary>
/// Defines the execution order phase for entity plugins.
/// Plugins in earlier phases are applied before later ones.
/// Used during entity construction and plugin sorting.
/// </summary>
public enum PluginExecutionPhase
{
    /// <summary>
    /// Setup context or shared blackboard data. Always first.
    /// </summary>
    Context = 0,

    /// <summary>
    /// Pure data or stat injection (e.g., HP, StatusEffects).
    /// </summary>
    Data = 1,

    /// <summary>
    /// Core movement systems (e.g., NavMesh, Rigidbody).
    /// </summary>
    Movement = 2,

    /// <summary>
    /// Behavior tree nodes that need movement/logic setup.
    /// </summary>
    BtExecution = 3,

    /// <summary>
    /// Time-driven logic (e.g., Pause, Timeout).
    /// </summary>
    TimedExecution = 4,

    /// <summary>
    /// Visuals, VFX, animation overrides.
    /// </summary>
    Visuals = 5,

    /// <summary>
    /// Audio setup or playback configuration.
    /// </summary>
    Audio = 6,

    /// <summary>
    /// Final stage: modifiers, effects, optional helpers.
    /// </summary>
    Post = 7
}