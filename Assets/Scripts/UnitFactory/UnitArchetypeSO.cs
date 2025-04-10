using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UnitFactory
{
    [CreateAssetMenu(fileName = "UnitArchetypeSO", menuName = "Scriptable Objects/UnitArchetypeSO")]
    public class UnitArchetypeSO : ScriptableObject
    {
        public GameObject Prefab;
        public BTNodeConfig BehaviorTree;
        public List<ScriptableObject> AttributeModules;
        // private publicList<SkillSO> Skills; // Optional
    }
}