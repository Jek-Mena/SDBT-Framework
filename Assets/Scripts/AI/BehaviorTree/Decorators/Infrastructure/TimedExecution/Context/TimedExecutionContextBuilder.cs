using UnityEngine;

public class TimedExecutionContextBuilder : IContextBuilderModule
{
    public void Build(GameObject entity, Blackboard blackboard)
    {
        var timer = entity.GetComponent<ITimedExecutionNode>();
        if (timer == null)
        {
            Debug.LogError($"[TimedExecutionContextBuilder] Missing ITimedExecutionNode on {entity.name}");
            return;
        }

        blackboard.TimedExecutionLogic = timer;
        Debug.Log($"[TimedExecutionContextBuilder] Bound TimedExecutionLogic for {entity.name}");
    }
}