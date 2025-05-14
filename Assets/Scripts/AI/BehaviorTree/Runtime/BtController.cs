using System.Collections.Generic;
using UnityEngine;

public class BtController : MonoBehaviour
{
    public Blackboard Blackboard;
    private IBehaviorNode _rootNode;

    public void InitContext(Blackboard blackboard) => Blackboard = blackboard;

    public void LoadBtFromRunTime(IBehaviorNode rootNode) => _rootNode = rootNode;
    
    private void Update()
    {
        if (_rootNode == null) return;

        var result = _rootNode.Tick(this);
        Debug.Log($"[BT Tick] Status: {result}");
    }
}