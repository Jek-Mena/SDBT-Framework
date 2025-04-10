using System;
using System.Collections.Generic;

public class AttributeContainer
{
    private readonly Dictionary<Type, object> _attributes = new();

    public void Inject<T>(T data) where T : class
    {
        var type = data.GetType();
        var interfaces = type.GetInterfaces();

        foreach (var iface in interfaces)
        {
            if (!_attributes.ContainsKey(iface))
            {
                _attributes[iface] = data;
            }
        }

        _attributes[type] = data;
    }

    public bool TryGet<T>(out T data) where T : class
    {
        if (_attributes.TryGetValue(typeof(T), out var obj) && obj is T casted)
        {
            data = casted; 
            return true;
        }

        data = null;
        return false;
    }

    public bool TryGet(Type type, out object data) => _attributes.TryGetValue(type, out data);
}