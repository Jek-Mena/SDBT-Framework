using UnityEngine;

public static class ComponentUtils
{
    /// <summary>
    /// Ensures that the specified component of type <typeparamref name="T"/> exists on the given <see cref="GameObject"/>.
    /// If the component is not found, a <see cref="MissingComponentException"/> is thrown.
    /// </summary>
    /// <typeparam name="T">The type of the component to retrieve.</typeparam>
    /// <param name="obj">The <see cref="GameObject"/> to search for the component.</param>
    /// <returns>The component of type <typeparamref name="T"/> attached to the <paramref name="obj"/>.</returns>
    /// <exception cref="MissingComponentException">
    /// Thrown if the specified component of type <typeparamref name="T"/> is not found on the <paramref name="obj"/>.
    /// </exception>
    public static T RequireComponent<T>(this GameObject obj) where T : Component
    {
        var comp = obj.GetComponent<T>();
        if (!comp)
            throw new MissingComponentException($"[{typeof(T).Name}] Expected component of type {typeof(T).Name} on {obj.name}, but none was found.");
        return comp;
    }

    /// <summary>
    /// Ensures that the specified component of type <typeparamref name="T"/> exists on the given <see cref="Component"/>.
    /// If the component is not found, a <see cref="MissingComponentException"/> is thrown.
    /// </summary>
    /// <typeparam name="T">The type of the component to retrieve.</typeparam>
    /// <param name="context">The <see cref="Component"/> to search for the specified component.</param>
    /// <returns>The component of type <typeparamref name="T"/> attached to the <paramref name="context"/>.</returns>
    /// <exception cref="MissingComponentException">
    /// Thrown if the specified component of type <typeparamref name="T"/> is not found on the <paramref name="context"/>.
    /// </exception>
    public static T RequireComponent<T>(this Component context) where T : Component
    {
        var comp = context.GetComponent<T>();
        if (!comp)
            throw new MissingComponentException($"[{typeof(T).Name}] Expected component of type {typeof(T).Name} on {context.gameObject.name}, but none was found.");
        return comp;
    }
}