using System.Collections.Generic;
using UnityEngine;

namespace AI.GroupAI.SquadAgent
{
    public class SquadManager
    {
        public readonly List<ISquadAgent> Agents = new();
        public SquadFormationType FormationType = SquadFormationType.Matrix2x2;
        public ISquadAgent Leader => Agents.Count > 0 ? Agents[0] : null;
        public Vector3 GoalPosition;
        
        // TODO for below
        // Add Inspector Editability (Unity/ScriptableObject Approach, Optional but Highly Recommended)
        // Create a FormationConfig ScriptableObject for inspector editing, or
        // Use Odin Inspector or a custom editor if you want real designer flexibility.
        // At runtime, serialize SO to JSON (for hot-reloading) or load JSON as a text asset and inject into managers.
        private FormationParameters _formationParameters = new FormationParameters
        {
            Rows = 2,
            Columns = 2,
            Spacing = 1.5f,
            RoleAssignments = new Dictionary<int, string>()
        };
        public FormationParameters FormationParameters => _formationParameters;
        
        public void SetFormationParameters(FormationParameters parameters)
        {
            _formationParameters = parameters;
            UpdateFormation();
        }
        
        // Called when agent list or formation type changes
        public void UpdateFormation()
        {
            var generator = FormationGeneratorRegistry.GetGenerator(FormationType);
            var slots = generator.GenerateFormation(_formationParameters, Agents.Count);

            if (slots.Count < Agents.Count)
                Debug.LogWarning($"Not enough slots generated for agents: {slots.Count} < {Agents.Count}");
            
            for (var i = 0; i < Agents.Count; i++)
            {
                Agents[i].SetSquadSlot(slots[i], this);
            }
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