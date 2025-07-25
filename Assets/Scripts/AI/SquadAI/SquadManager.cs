using System.Collections.Generic;
using UnityEngine;

namespace AI.SquadAI
{
    public class SquadManager
    {
        public List<ISquadAgent> Agents = new();
        public SquadFormationType FormationType = SquadFormationType.Matrix2x2;
        public ISquadAgent Leader => Agents.Count > 0 ? Agents[0] : null;
        public Vector3 GoalPosition;
        
        // Called when agent list or formation type changes
        public void UpdateFormation()
        {
            // Get formation slots relative to leader
            var slots = GetFormationSlots(FormationType, Agents.Count);

            for (var i = 0; i < Agents.Count; i++)
            {
                Agents[i].SetSquadSlot(slots[i], this);
            }
        }
        
        public static List<Formation> GetFormationSlots(SquadFormationType formation, int count)
        {
            var slots = new List<Formation>();
            switch (formation)
            {
                case SquadFormationType.Matrix2x2:
                    // 2x2 matrix: positions relative to leader (front)
                    // Leader at (0,0), right, left, back
                    slots.Add(new Formation { AgentIndex = 0, Offset = new Vector3(0, 0, 0) }); // Leader
                    slots.Add(new Formation { AgentIndex = 1, Offset = new Vector3(-1, 0, -1) });
                    slots.Add(new Formation { AgentIndex = 2, Offset = new Vector3(1, 0, -1) });
                    slots.Add(new Formation { AgentIndex = 3, Offset = new Vector3(0, 0, -2) });
                    break;
                case SquadFormationType.Triangle:
                    slots.Add(new Formation { AgentIndex = 0, Offset = new Vector3(0, 0, 0) }); // Leader
                    slots.Add(new Formation { AgentIndex = 1, Offset = new Vector3(-1, 0, -1) });
                    slots.Add(new Formation { AgentIndex = 2, Offset = new Vector3(1, 0, -1) });
                    break;
                case SquadFormationType.Blob:
                    for (int i = 0; i < count; i++)
                        slots.Add(new Formation { AgentIndex = i, Offset = Vector3.zero }); // All same goal
                    break;
            }
            return slots;
        }
        
        public void AddAgent(ISquadAgent agent)
        {
            if (Agents.Contains(agent)) return;
            Agents.Add(agent);
            UpdateFormation();
        }

        public void RemoveAgent(ISquadAgent agent)
        {
            if (Agents.Remove(agent))
                UpdateFormation();
        }

        public void SetGoal(Vector3 position)
        {
            GoalPosition = position;
            UpdateFormation();
        }
    }
}