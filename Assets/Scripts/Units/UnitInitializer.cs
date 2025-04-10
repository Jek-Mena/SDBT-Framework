using Assets.Scripts.Shared.AI;
using Assets.Scripts.UnitFactory;
using UnityEngine;

public class UnitInitializer : MonoBehaviour
{
    public void InitializeFromArchetype(UnitArchetypeSO archetypeSo)
    {
        var container = new AttributeContainer();

        foreach (var so in archetypeSo.AttributeModules)
        {
            if (so is not IUnitAttributesSO module) continue;

            Debug.Log($"Injecting attribute module: {so.name} into {gameObject.name}");
            var runtime = module.CreateRuntime();
            container.Inject(runtime);
        }

        foreach (var component in GetComponents<MonoBehaviour>())
        {
            DependencyInjector.InjectDependencies(component, container);
        }

        // Inject into behavior tree
        if (!TryGetComponent(out NPCBehaviorTreeController bt) || archetypeSo.BehaviorTree == null) return;

        var coordinator = GetComponent<UnitCoordinator>();
        var context = new BTNodeContext(coordinator, container);
        bt.LoadTree(archetypeSo.BehaviorTree, context);
    }
}