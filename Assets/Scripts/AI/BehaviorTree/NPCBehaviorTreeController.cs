using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviorTreeController : MonoBehaviour
{
    [Tooltip("Attached a composite node config (Sequence, etc.)")] 
    [SerializeField] private BTNodeConfig _behaviorTreeRoot;

    private IBehaviorNode _rootNode;

    private void Update()
    {
        _rootNode?.Tick(this);
    }

    public void SetRootNode(BTNodeConfig btRootNode)
    {
        _behaviorTreeRoot = btRootNode;
    }

    public void LoadTree(BTNodeConfig root, BTNodeContext context)
    {
        _behaviorTreeRoot = root;
        _rootNode = _behaviorTreeRoot?.CreateNode(context);
    }

    public UnitCoordinator Coordinator { get; set; }
    public bool IsPhysicsOverridingMovement { get; set; }
    public bool WasInterrupted { get; set; } //TODO separate CombatState or StatusComponent, and let the BT access it via npc.Status.WasInterrupted.

} 

