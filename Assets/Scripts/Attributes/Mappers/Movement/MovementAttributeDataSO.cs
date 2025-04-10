using System.Collections.Generic;
using Assets.Scripts.UnitFactory;
using UnityEngine;

[CreateAssetMenu(menuName = "Attributes/Movement Attribute")]
public class MovementAttributeDataSO : ScriptableObject, IUnitAttributesSO
{
    public List<NamedFloatField<MovementFieldKey>> Fields;
    public MovementAttributeMapper Mapper;

    public IUnitAttributeRuntime CreateRuntime()
    {
        if (Mapper != null) return Mapper.MapFrom(Fields);

        Debug.LogError($"Mapper not assigned in {name}.");
        return null;
    }
}