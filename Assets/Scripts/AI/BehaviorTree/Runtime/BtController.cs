using UnityEngine;

public class BtController : MonoBehaviour
{
    public BtContext Context;
    public Blackboard Blackboard;
    private IBehaviorNode _rootNode;
    
    public void InitContext(BtContext context)
    {
        Context = context;
        Blackboard = context.Blackboard;
    }

    public void SetTree(IBehaviorNode rootNode) => _rootNode = rootNode;
    
    private void Update()
    {
        if (_rootNode == null) return;

        var result = _rootNode.Tick(Context);
        Debug.Log($"[BT Tick] Status: {result}");
    }
}