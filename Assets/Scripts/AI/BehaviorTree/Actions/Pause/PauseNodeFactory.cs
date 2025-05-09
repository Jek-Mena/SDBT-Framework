public class PauseNodeFactory : TimedExecutionNodeFactory<PauseNode>
{
    public PauseNodeFactory() : base(JsonLiterals.Behavior.TimedExecution.Pause)
    {

    }
}