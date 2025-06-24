using Newtonsoft.Json.Linq;
using UnityEngine;

public class BtController : MonoBehaviour
{
    [Header("Behavior Tree Switching")]
    [Tooltip("Plug in a switching strategy (MonoBehaviour implementing IBehaviorTreeSwitcher).")]
    [SerializeField] private StimuliSwitcher switcherComponent;
    private IBehaviorTreeSwitcher _switcher;
    private string _activeTreeKey;
    
    public BtContext Context;
    public Blackboard Blackboard;
    private IBehaviorNode _rootNode;
    
    private void Awake()
    {
        _switcher = switcherComponent;
        if (_switcher != null)
            _switcher.OnSwitchRequested += OnSwitchRequested;
        else
            Debug.LogError("No IBehaviorTreeSwitcher component found on " + name);
    }

    private void OnSwitchRequested(string fromKey, string toKey, string reason)
    {
        if (toKey != _activeTreeKey)
        {
            SwitchToTree(toKey, $"event: {reason}");
        }
    }
    
    public void SwitchToTree(string treeKey, string reason)
    {
        Debug.Log($"[BtController] Switching tree: {_activeTreeKey ?? "(none)"} -> {treeKey} (reason: {reason})");

        // Retrieve template (unresolved) from registry
        var btJsonTemplate = BtRegistry.GetTemplate(treeKey);
        if (btJsonTemplate == null)
        {
            Debug.LogError($"[BtController] Failed to switch—tree key '{treeKey}' not found in registry.");
            return;
        }
        
        // Deep clone for isolation
        var agentBtJson = btJsonTemplate.DeepClone() as JObject;
        
        // Use current agent's context (must be up to date)
        var rootNode = BtTreeBuilder.LoadFromToken(agentBtJson, Context);
        
        // Assign to controller
        SetTree(rootNode);
        _activeTreeKey = treeKey;
        Debug.Log($"[BtController] Successfully switched to BT '{treeKey}'");
    }
    
    public void InitContext(BtContext context)
    {
        Context = context;
        Blackboard = context.Blackboard;
    }

    public void SetTree(IBehaviorNode rootNode) => _rootNode = rootNode;

    private void Update()
    {
        if (_switcher != null && Context != null)
        {
            // TODO - add support for polling(not sure?) but deal eventually deal with the Expensive Invocation
            var newKey = _switcher.EvaluateSwitch(Context);
            if (!string.IsNullOrEmpty(newKey) && newKey != _activeTreeKey)
            {
                SwitchToTree(newKey, "polled switcher");
            }
        }

        if (_rootNode != null && Context != null)
        {
            var result = _rootNode.Tick(Context);
            Debug.Log($"[BT Tick] Status: {result}");
        }
    }
}