using System;
using System.Collections.Generic;
using UnityEngine;

// Repeat similarly for HealthAttributeDataSO with HealthFieldKey and HealthAttributes.

[CreateAssetMenu(menuName = "Attributes/Mapper/Movement")]
public class MovementAttributeMapper : ScriptableObject
{
    public MovementAttributes MapFrom(List<NamedFloatField<MovementFieldKey>> fields)
    {
        var attr = new MovementAttributes();

        foreach (var field in fields)
        {
            switch (field.Key)
            {
                case MovementFieldKey.Speed: attr.Speed = field.Value; break;
                case MovementFieldKey.Acceleration: attr.Acceleration = field.Value; break;
                case MovementFieldKey.StoppingDistance: attr.StoppingDistance = field.Value; break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return attr;
    }
}
