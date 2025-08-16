using System.Collections.Generic;
using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Registry;
using Systems.FearPerception;
using Utils;

namespace AI.BehaviorTree.Runtime.Context
{
    public static class FearBlackboardWriter
    {
        /// <summary>
        /// Copy stimuli into the agent’s stable list (fixed capacity), update fast flag,
        /// and mirror IDs (and optional aggregate) into POD (BlackboardData).
        /// </summary>
        public static void UpdateStimuli(
            IReadOnlyList<FearStimulus> source,      // temp list OR direct collector
            Blackboard blackboard,
            ref BlackboardData bbData)
        {
            // 1) Stable managed list on BB with FIXED capacity
            var dest = blackboard.GetListForWriteFixed(BlackboardKeys.Fear.StimuliNearby, BbConstants.FEAR_STIMULI_MAX);
            dest.Clear();

            // 2) Copy up to MAX, never grow
            var count = source != null ? source.Count : 0;
            if (count > BbConstants.FEAR_STIMULI_MAX) count = BbConstants.FEAR_STIMULI_MAX;

            var totalIntensity = 0f;
            for (var i = 0; i < count; i++)
            {
                if (source == null) continue;
                var s = source[i];
                dest.Add(s);
                //totalIntensity += s.Intensity; // if you have it
            }

            // 3) Fast boolean flag (no managed access required for checks)
            bbData.SetFlag((byte)BbFlags.HasFearStimuli, count > 0);

            // 4) POD mirror for ECS (IDs only → FixedList)
            bbData.FearStimuliIds.Length = 0;
            var cap = bbData.FearStimuliIds.Capacity;
            // FixedList is small; clamp to its capacity too
            var podCount = count < cap ? count : cap;
            for (var i = 0; i < podCount; i++)
            {
                var id = RuntimeIdRegistry.GetId(dest[i]);
                bbData.AddFearStimulus(id);
            }

            // Optional aggregate stored in POD (cheap read later)
            bbData.FearStimulusLevel = totalIntensity;
        }
    }
}