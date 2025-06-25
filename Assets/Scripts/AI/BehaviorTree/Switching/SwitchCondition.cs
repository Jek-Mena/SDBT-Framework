using System;

[Serializable]
public class SwitchCondition
{
    public string stimulusKey;
    public string comparisonOperator; // "LessThan", "GreaterThan", "Equal", "NotEqual", etc.
    public float threshold;
    public string behaviorTree;
}