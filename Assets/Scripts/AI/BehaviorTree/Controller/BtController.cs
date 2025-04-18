using System.Collections.Generic;
using UnityEngine;

public class BtController : MonoBehaviour
{
    private IBehaviorNode _rootNode;
    private Blackboard _context;
    
    public void InitContext(Blackboard context) => _context = context;

    public void LoadBtFromRunTime(IBehaviorNode rootNode) => _rootNode = rootNode;
    
    public void AddAbilityNode(string ability)
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        if (_rootNode == null) return;

        var result = _rootNode.Tick(this);
        Debug.Log($"[BT Tick] Status: {result}");
    }
}

