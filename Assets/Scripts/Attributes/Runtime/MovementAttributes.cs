public class MovementAttributes : IMovementAttributes, IUnitAttributeRuntime
{
    public float Speed { get; set; }
    public float Acceleration { get; set; }
    public float StoppingDistance { get; set; }
}