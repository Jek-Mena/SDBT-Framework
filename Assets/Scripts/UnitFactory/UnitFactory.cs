using Assets.Scripts.UnitFactory;
using UnityEngine;

public class UnitFactory : MonoBehaviour, IUnitFactory
{
    public GameObject SpawnUnit(UnitArchetypeSO archetypeSo, Vector3 position)
    {
        var tempGameObject = Instantiate(archetypeSo.Prefab, position, Quaternion.identity);
        var initializer = tempGameObject.GetComponent<UnitInitializer>();
        initializer.InitializeFromArchetype(archetypeSo);
        return tempGameObject;
    }
}