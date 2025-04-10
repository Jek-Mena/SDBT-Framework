using UnityEngine;

public static class DependencyInjector
{
    public static void InjectDependencies(object target, AttributeContainer container)
    {
        var targetType = target.GetType();
        var interfaces = targetType.GetInterfaces();

        foreach (var iface in interfaces)
        {
            if(!iface.IsGenericType || iface.GetGenericTypeDefinition() != typeof(IRequire<>))
                continue;

            var dependencyType = iface.GetGenericArguments()[0];
            
            if (!container.TryGet(dependencyType, out var dependency))
            {
                Debug.LogWarning($"[DI] Missing dependency {dependencyType.Name} for {targetType.Name}");
                continue;
            }

            var method = iface.GetMethod("Inject");
            if (method == null)
            {
                Debug.LogError($"[DI] Could not find Inject() method on {targetType.Name} for {dependencyType.Name}");
                continue;
            }
            
            method.Invoke(target, new[] { dependency });
            Debug.Log($"[DI] Injected {dependencyType.Name} into {targetType.Name}");
        }
    }
}