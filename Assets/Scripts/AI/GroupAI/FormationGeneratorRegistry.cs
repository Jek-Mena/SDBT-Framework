using System.Collections.Generic;
using AI.GroupAI.SquadAgent;

namespace AI.GroupAI
{
    public class FormationGeneratorRegistry
    {
        private static readonly Dictionary<SquadFormationType, IFormationGenerator> Generators = new()
        {
            { SquadFormationType.Matrix2x2, new MatrixFormationGenerator() }
        };
        
        public static IFormationGenerator GetGenerator(SquadFormationType formationType)
        {
            if (!Generators.TryGetValue(formationType, out var generator))
                throw new System.Exception($"[FormationGeneratorRegistry] No generator found for type: {formationType}");
            return generator;
        }
    }
}