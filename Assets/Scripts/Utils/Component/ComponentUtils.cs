using System;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ComponentUtils
{
    public static T RequireComponent<T>(this GameObject obj) where T : Component
    {
        var comp = obj.GetComponent<T>();
        if (comp == null)
            throw new MissingComponentException($"Expected component of type {typeof(T).Name} on {obj.name}, but none was found.");
        return comp;
    }

    public static T RequireComponent<T>(this Component context) where T : Component
    {
        var comp = context.GetComponent<T>();
        if (comp == null)
            throw new MissingComponentException($"Expected component of type {typeof(T).Name} on {context.gameObject.name}, but none was found.");
        return comp;
    }
}