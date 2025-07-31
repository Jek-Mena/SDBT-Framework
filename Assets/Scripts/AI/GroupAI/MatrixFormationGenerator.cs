using System.Collections.Generic;
using UnityEngine;

namespace AI.GroupAI
{
    public class MatrixFormationGenerator : IFormationGenerator
    {
        private const int MaxRows = 5;
        private const int MaxCols = 5;
        
        public List<Formation> GenerateFormation(FormationParameters parameters, int agentCount)
        {
            var rows = parameters.Rows;
            var cols = parameters.Columns;
            var spacing = parameters.Spacing;
            
            var slots = new List<Formation>(agentCount);
            
            // Flatten all possible positions in a row-major order
            var offsets = new List<Vector3>();
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    offsets.Add(new Vector3(
                        (c - (cols - 1) / 2.0f) * spacing,
                        0,
                        (r - (rows - 1) / 2.0f) * spacing
                    ));
                }
            }
            
            // Use as many offsets as agents (if not enough, clamp)
            var slotsToFill = Mathf.Min(agentCount, offsets.Count);
           
            // Default: first agent is leader
            for (var i = 0; i < slotsToFill; i++)
                slots.Add(new Formation { AgentSlotIndex = i, Offset = offsets[i] });

            // Determine leader placement
            if (slotsToFill > 0)
            {
                var desiredPos = Vector3.zero;

                switch (parameters.Behavior)
                {
                    case FormationBehavior.Squad.LeaderInFront:
                        desiredPos = new Vector3(0, 0, ((0 - (rows - 1) / 2.0f) * spacing)); // front middle
                        break;

                    case FormationBehavior.Squad.SurroundLeader:
                        desiredPos = new Vector3(0, 0, 0);
                        break;

                    case FormationBehavior.Squad.LeaderInRear:
                        desiredPos = new Vector3(0, 0, ((rows - 1) - (rows - 1) / 2.0f) * spacing);
                        break;
                }

                var leaderOffset = FindClosestOffset(offsets.GetRange(0, slotsToFill), desiredPos);
                if (leaderOffset.HasValue)
                {
                    var leaderIdx = slots.FindIndex(f => f.Offset == leaderOffset.Value);
                    if (leaderIdx >= 0)
                    {
                        (slots[0], slots[leaderIdx]) = (slots[leaderIdx], slots[0]);
                    }
                }
            }
            return slots;
        }
        
        private Vector3? FindClosestOffset(List<Vector3> offsets, Vector3 target)
        {
            var minDist = float.MaxValue;
            Vector3? best = null;

            foreach (var offset in offsets)
            {
                var dist = Vector3.SqrMagnitude(offset - target);
                if (!(dist < minDist)) continue;
                minDist = dist;
                best = offset;
            }
            return best;
        }
    }
}