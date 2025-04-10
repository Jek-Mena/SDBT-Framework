using Assets.Scripts.UnitFactory;
using UnityEngine;

public class SpawnEnemyWithBT : MonoBehaviour
{
    public UnitFactory Factory;
    public UnitArchetypeSO Archetype;
    public int Count = 1;
    public float Radius = 5f;

    private void Start()
    {
        for (int i = 0; i < Count; i++)
        {
            var pos = transform.position + Random.insideUnitSphere * Radius;
            pos.y = 0;
            Factory.SpawnUnit(Archetype, pos);
        }
    }
}