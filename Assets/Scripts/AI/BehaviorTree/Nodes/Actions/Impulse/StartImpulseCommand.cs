using UnityEngine;

public class StartImpulseCommand : IFixedUpdateAction
{
    private readonly IImpulseNode _dash;
    private readonly Vector3 _direction;

    public StartImpulseCommand(IImpulseNode dash, Vector3 direction)
    {
        _dash = dash;
        _direction = direction;
    }

    public void Execute()
    {
        _dash.PerformImpulse(_direction);
    }
}