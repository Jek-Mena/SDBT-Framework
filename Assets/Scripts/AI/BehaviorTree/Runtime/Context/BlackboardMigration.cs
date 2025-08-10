using System;
using UnityEngine;

namespace AI.BehaviorTree.Runtime.Context
{
    using Unity.Mathematics;
    using UnityEngine;
    using AI.BehaviorTree.Keys;

    public static class BlackboardMigration
    {
        public static void MigrateSet(Blackboard blackboard, string key, object value)
        {
            ref var data = ref blackboard.DataRef;
            switch (key)
            {
                case BlackboardKeys.Fear.CurrentLevel:       // "CurrentFearLevel"
                    data.FearCurrentLevel = Convert.ToSingle(value);
                    break;

                case BlackboardKeys.Fear.StimulusLevel:      // "FearStimulusLevel"
                    data.FearStimulusLevel = Convert.ToSingle(value);
                    break;

                case BlackboardKeys.Target.CurrentTarget:
                    if (value is GameObject go)
                        data.CurrentTargetId = go.GetInstanceID();
                    else if (value is Transform t)
                        data.CurrentTargetId = t.gameObject.GetInstanceID();
                    else if (value is Vector3 v3)
                        data.FormationPosition = new float3(v3.x, v3.y, v3.z);
                    break;

                case BlackboardKeys.Target.Formation:
                    if (value is Vector3 fp)
                        data.FormationPosition = new float3(fp.x, fp.y, fp.z);
                    break;

                case BlackboardKeys.Multipliers.Armor:
                    data.ArmorMultiplier = Convert.ToSingle(value);
                    break;

                case BlackboardKeys.Multipliers.Attack:
                    data.AttackMultiplier = Convert.ToSingle(value);
                    break;

                case BlackboardKeys.Multipliers.Movement:
                    data.MovementMultiplier = Convert.ToSingle(value);
                    break;

                // TODO: add any other keys you actually use in runtime
            }
        }
    }
}