using System;

[AttributeUsage(AttributeTargets.Class)]
public class BtNodeAttribute : Attribute
{
    public string NodeType { get; }

    public BtNodeAttribute(string nodeType)
    {
        NodeType = nodeType;
    }
}