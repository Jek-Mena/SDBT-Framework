// The variable name here should be the same in the JSON file
public class MovementData
{
    public float Speed;
    public float AngularSpeed;
    public float Acceleration;
    public float StoppingDistance;
    public float UpdateThreshold = 0f; // default to "always update"
    public Direction Direction = Direction.Forward; // Optional for certain Movement (e.g. Transform)
}

public enum Direction
{
    Forward,
    Backward,
    Left,
    Right,
    Up,
    Down,
    ForwardLeft,
    ForwardRight,
    BackwardLeft,
    BackwardRight,
}