using System.Reflection;
using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public abstract class BasePlugin : IEntityComponentPlugin
{
    public PluginKey Key => GetType().GetCustomAttribute<PluginAttribute>()?.Key ?? throw new Exception("Plugin missing [Plugin] attribute.");
    public abstract void Apply(GameObject entity, JObject jObject);
}