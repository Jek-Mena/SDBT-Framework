using System;

public class MovementSettings
{
    public float? Speed;
    public float? AngularSpeed;
    public float? Acceleration;
    public float? StoppingDistance;
    public bool IsControlled; // Optional control flag to halt movement such as Freeze, Stun, etc.

    public MovementSettings(){ }

    public MovementSettings(MovementSettings copy)
    {
        if (copy == null)
            throw new ArgumentNullException(nameof(copy), "Cannot copy from null MovementSettings.");

        Speed = copy.Speed;
        AngularSpeed = copy.AngularSpeed;
        Acceleration = copy.Acceleration;
        StoppingDistance = copy.StoppingDistance;
        IsControlled = copy.IsControlled;
    }
}