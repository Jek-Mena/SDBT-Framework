using System.Collections.Generic;

namespace AI.GroupAI
{
    public interface IFormationGenerator
    {
        List<Formation> GenerateFormation(FormationParameters parameters, int agentCount);
    }
}