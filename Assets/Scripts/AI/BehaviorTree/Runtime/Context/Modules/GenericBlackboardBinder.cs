using System;
using UnityEngine;

public class GenericBlackboardBinder<TComponent, TSettings, TProvider> : IContextBuilderModule
    where TComponent : Component, IAcceptsSettingsProvider<TSettings>
    where TProvider : ISettingsProvider<TSettings>, new()
{
    private readonly string _providerKey;
    private readonly Action<Blackboard, TComponent> _staticFieldBinder;

    public GenericBlackboardBinder(string providerKey, Action<Blackboard, TComponent> staticFieldBinder)
    {
        _providerKey = providerKey;
        _staticFieldBinder = staticFieldBinder;
    }

    public void Build(GameObject entity, Blackboard blackboard)
    {
        if (!entity.TryGetComponent<TComponent>(out var component))
        {
            Debug.LogWarning($"[GenericBlackboardBinder] {typeof(TComponent).Name} not found on {entity.name}");
            return;
        }

        var provider = new TProvider();
        component.SetSettingsProvider(provider);

        // Always put provider in dynamic store
        blackboard.Set(_providerKey, provider);

        // Bind to a static field (e.g., MovementLogic)
        _staticFieldBinder.Invoke(blackboard, component);

        Debug.Log($"[GenericBlackboardBinder] Injected {typeof(TProvider).Name} into {typeof(TComponent).Name}, key = {_providerKey}");
    }
}