using UnityEngine;

namespace Assets.Scripts.UnitFactory
{
    public class UnitInitializer : MonoBehaviour
    {
        public void InitializeFromArchetype(UnitArchetypeSO archetypeSo)
        {
            var container = new AttributeContainer();

            foreach (var module in archetypeSo.AttributeModules)
            {
                if(module == null) continue;

                if (module is not IUnitAttributesSO attributeSO)
                {
                    Debug.LogWarning($"{module.name} does not implement IUnitAttributesSO");
                    continue;
                }

                var runtime = attributeSO.CreateRuntime();
                Debug.Log($"[ATTR] Created runtime: {runtime.GetType().Name} from {module.GetType().Name}");
                container.Inject(runtime);
            }

            // InjectDependencies into systems
            foreach (var component in GetComponents<MonoBehaviour>())
            {
                DependencyInjector.InjectDependencies(component, container);
            }

            // Behavior Tree, skills, etc.
            if (TryGetComponent(out NPCBehaviorTreeController npcBtc) && archetypeSo.BehaviorTree != null)
            {
                var context = new BTNodeContext(GetComponent<UnitCoordinator>(), container);
                npcBtc.LoadTree(archetypeSo.BehaviorTree, context);
            }
        }
    }
}