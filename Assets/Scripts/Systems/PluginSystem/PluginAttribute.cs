using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PluginAttribute : Attribute
{
    public PluginKey Key { get; }

    public PluginAttribute(PluginKey key) => Key = key;

}