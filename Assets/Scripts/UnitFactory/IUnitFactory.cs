using Assets.Scripts.UnitFactory;
using UnityEngine;

public interface IUnitFactory
{
    GameObject SpawnUnit(UnitArchetypeSO archetypeSo, Vector3 position);
}