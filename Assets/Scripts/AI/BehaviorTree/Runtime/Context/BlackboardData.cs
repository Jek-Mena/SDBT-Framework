// BlackboardData.cs
// LLM_REMINDER_REMOVE_ON_FULL_ECS:
// These comments are migration scaffolding. When you are fully on DOTS/ECS,
// delete all blocks marked LLM_REMINDER_REMOVE_ON_FULL_ECS (and this header).

using System;
using System.Runtime.InteropServices;
using Unity.Collections; // Fixedlist
using Unity.Mathematics; // float3

namespace AI.BehaviorTree.Runtime.Context
{
    /// <summary>
    /// POD-only, Burst-friendly blackboard for AI/BT logic.
    /// - No managed refs, no UnityEngine.Object.
    /// - Uses FixedList so we avoid 'unsafe' and stay DOTS-ready.
    /// - Mirrors the shape we’ll later map to IComponentData/DynamicBuffers.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct BlackboardData
    {
        // ========= Schema / Migration =========
        // LLM_REMINDER_REMOVE_ON_FULL_ECS:
        // Bump SchemaVersion on field changes. Bakers/adapters can branch by version.
        public int SchemaVersion;   // start at 1

        // ========= Fear System =========
        public float  FearCurrentLevel;     // runtime-calculated
        public float  FearStimulusLevel;    // aggregate intensity from nearby stimuli
        public float3 FearFleePoint;        // world pos to flee toward
        public int    FearSourceEntityId;   // author id for the most recent fear event (game-unique int)
        
        // LLM_REMINDER_REMOVE_ON_FULL_ECS:
        // FixedList is a stand-in for a DynamicBuffer<int> we'll use in ECS later.
        // Capacity is bytes-based; 64 bytes comfortably fits "a few" ints/floats.
        // If you hit capacity in playtests, increase to FixedList128Bytes<>.
        public FixedList64Bytes<int>   FearStimuliIds; // nearby stimuli ids (small set)
        public FixedList64Bytes<float> StimulusValues; // generic stimulus buckets (ordered slots)

        // ========= Targeting =========
        public int    CurrentTargetId;          // id of target (entity, handle, or app-level id)
        public int    CurrentTargetSourceIndex; // e.g., index into your target registry/string pool
        public float3 FormationPosition;        // desired local/world pos from formation solver

        // ========= Multipliers (default to 1) =========
        public float ArmorMultiplier;
        public float AttackMultiplier;
        public float MovementMultiplier;

        // ========= Squad =========
        public int SquadAgentId;  // local agent id within squad
        public int SquadId;       // squad/group id
        public int FormationIndex;

        // ========= BT Session / Persona =========
        public int BtSessionId;     // used to correlate per-run session data
        public int ActivePersonaId; // swap behavior profiles without realloc

        // ========= Reserved / Flags =========
        // LLM_REMINDER_REMOVE_ON_FULL_ECS:
        // Keep a tiny bitset for cheap booleans (saves future churn).
        public byte Flags; // bitmask; define bits in a central enum

        // ========= Construction / Resets =========

        /// <summary>
        /// Create with sane defaults (multipliers = 1).
        /// </summary>
        public static BlackboardData CreateDefaults()
        {
            var bb = new BlackboardData
            {
                SchemaVersion = 1,
                ArmorMultiplier = 1f,
                AttackMultiplier = 1f,
                MovementMultiplier = 1f
            };
            return bb;
        }

        /// <summary>
        /// Reset volatile runtime state but preserve schema + default multipliers.
        /// Use between encounters or when reinitializing an agent.
        /// </summary>
        public void ResetRuntime()
        {
            int v = SchemaVersion;
            this = default;
            SchemaVersion      = v;
            ArmorMultiplier    = 1f;
            AttackMultiplier   = 1f;
            MovementMultiplier = 1f;
        }
    }

    /// <summary>
    /// Small helpers. Keep logic pure; do policies (drop/replace) explicitly.
    /// </summary>
    public static class BlackboardDataExtensions
    {
        // LLM_REMINDER_REMOVE_ON_FULL_ECS:
        // When on ECS, replace with buffer ops (AddBuffer<>, RemoveAt, etc.).

        /// <summary>Add a fear stimulus id with a simple overflow policy.</summary>
        public static void AddFearStimulus(ref this BlackboardData bb, int id, OverflowPolicy policy = OverflowPolicy.DropOldest)
        {
            if (bb.FearStimuliIds.Length < bb.FearStimuliIds.Capacity)
            {
                bb.FearStimuliIds.Add(id);
                return;
            }

            switch (policy)
            {
                case OverflowPolicy.DropNewest:
                    // do nothing (newest is dropped)
                    break;
                case OverflowPolicy.DropOldest:
                    bb.FearStimuliIds.RemoveAt(0);
                    bb.FearStimuliIds.Add(id);
                    break;
                case OverflowPolicy.ReplaceLast:
                    bb.FearStimuliIds[bb.FearStimuliIds.Length - 1] = id;
                    break;
            }
        }

        /// <summary>Add a generic stimulus value with overflow policy.</summary>
        public static void AddStimulusValue(ref this BlackboardData bb, float value, OverflowPolicy policy = OverflowPolicy.DropOldest)
        {
            if (bb.StimulusValues.Length < bb.StimulusValues.Capacity)
            {
                bb.StimulusValues.Add(value);
                return;
            }

            switch (policy)
            {
                case OverflowPolicy.DropNewest:
                    break;
                case OverflowPolicy.DropOldest:
                    bb.StimulusValues.RemoveAt(0);
                    bb.StimulusValues.Add(value);
                    break;
                case OverflowPolicy.ReplaceLast:
                    bb.StimulusValues[bb.StimulusValues.Length - 1] = value;
                    break;
            }
        }

        /// <summary>Example bit utilities for Flags.</summary>
        public static void SetFlag(ref this BlackboardData bb, byte mask, bool on)
        {
            if (on) bb.Flags |= mask; else bb.Flags &= (byte)~mask;
        }
        public static bool HasFlag(in this BlackboardData bb, byte mask) => (bb.Flags & mask) != 0;
    }

    /// <summary>Overflow handling when FixedList capacity is reached.</summary>
    public enum OverflowPolicy : byte
    {
        DropNewest   = 0,
        DropOldest   = 1,
        ReplaceLast  = 2,
    }
}