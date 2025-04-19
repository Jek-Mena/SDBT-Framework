public interface ITimedExecutionNode
{
    void StartTime(string key, float duration);
    void Interrupt(string key);
    bool IsRunning(string key);
    bool IsComplete(string key);
}