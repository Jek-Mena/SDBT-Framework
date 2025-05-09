// The variable name here should be the same in the JSON file
[System.Serializable]
public class MovementData
{
    public float Speed;
    public float AngularSpeed;
    public float Acceleration;
    public float StoppingDistance;
    public float UpdateThreshold = 0f; // default to "always update"
}