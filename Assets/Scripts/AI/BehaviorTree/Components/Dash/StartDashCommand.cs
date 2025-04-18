using UnityEngine;

public class StartDashCommand : IGameAction
{
    private readonly IDashBehavior _dash;
    private readonly Vector3 _target;

    public StartDashCommand(IDashBehavior dash, Vector3 target)
    {
        _dash = dash;
        _target = target;
    }

    public void Execute()
    {
        _dash.PerformDash(_target);
    }
}