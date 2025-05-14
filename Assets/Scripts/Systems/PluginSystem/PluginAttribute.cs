using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PluginAttribute : Attribute
{
    public string PluginKey { get; }

    public PluginAttribute(string pluginKey) => PluginKey = pluginKey;

}