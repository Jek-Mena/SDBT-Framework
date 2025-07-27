using System.Collections.Generic;
using AI.GroupAI.SquadAgent;

namespace AI.GroupAI
{
    // TODO might rename into MatrixFormationParameters if the only use case of this is for matrix.
    public class FormationParameters
    {
        public int Rows;
        public int Columns;
        public float Spacing;
        public string Behavior;
        public SquadRoles Role;
        public Dictionary<int, string> RoleAssignments;
    }
}