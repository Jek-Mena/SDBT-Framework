using AI.BehaviorTree.Loader;
using AI.BehaviorTree.Registry;
using AI.BehaviorTree.Runtime.Context;
using AI.BehaviorTree.Switching;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Utils.Component;

namespace AI.BehaviorTree.Runtime
{
    public class BtController : MonoBehaviour
    {
        private const string ScriptName = nameof(BtController);
        
        public BtContext Context;
        public IBehaviorNode RootNode { get; private set; }
        
        private IBtPersonaSwitcher _personaSwitcher;
        private string _activePersonaTreeKey;

        private int _btSessionId = 0;
        
        private void OnSwitchRequested(string fromKey, string toKey, string reason)
        {
            if (toKey != _activePersonaTreeKey)
                SwitchPersonaTree(toKey, $"event: {reason}");
        }
    
        // Switches to a new tree by key; this is the only tree assignment API.
        public void SwitchPersonaTree(string treeKey, string reason)
        {
            if (_activePersonaTreeKey == treeKey) return;
        
            if (string.IsNullOrEmpty(treeKey))
            {
                Debug.LogError("[BtController] treeKey is null or empty!");
                return;
            }
            _activePersonaTreeKey = treeKey;

            Debug.Log($"[BtController] Switching tree: {_activePersonaTreeKey ?? "(none)"} -> {treeKey} (reason: {reason})");

            // Retrieve template (unresolved) from registry
            var btJsonTemplate = BtConfigRegistry.GetTemplate(treeKey);
            Debug.Log($"[DEBUG] Requested BT '{treeKey}' - Registry has: [{string.Join(", ", BtConfigRegistry.GetAllKeys())}]");
            if (btJsonTemplate == null)
            {
                Debug.LogError($"[BtController] Failed to switch—tree key '{treeKey}' not found in registry.");
                return;
            }
        
            // Deep clone for isolation
            var agentBtJson = btJsonTemplate.DeepClone() as JObject;
        
            // Use current agent's context (must be up to date)
            var rootNode = BtTreeBuilder.LoadTreeFromToken(agentBtJson, Context);

            // Bump the session
            _btSessionId++;
            Context.Blackboard.BtSessionId = _btSessionId;
            
            Context.Blackboard.MovementOrchestrator.TakeOwnership(Context.Blackboard.BtSessionId);
            
            // Assign to controller
            SetTree(rootNode);
            Debug.Log($"[BtController] Successfully switched to BT '{treeKey}'");
        }
    
        public void Initialize(BtContext context)
        {
            Context = context;

            _personaSwitcher = context.Blackboard.PersonaSwitcher;
            if (_personaSwitcher != null)
                _personaSwitcher.OnSwitchRequested += OnSwitchRequested;
            else
                Debug.LogError("No IBehaviorTreeSwitcher component found on " + name);

            if (_personaSwitcher != null)
            {
                Debug.Log($"[{ScriptName}] Start() called on {gameObject.name}. Context: {Context != null}. PersonaSwitcher: {_personaSwitcher != null}");
                var initialPersonaKey = _personaSwitcher.EvaluateSwitch(Context, _activePersonaTreeKey);
                if (!string.IsNullOrEmpty(initialPersonaKey))
                    SwitchPersonaTree(initialPersonaKey, "Initial switcher based on stimuli.");
            }
            else
            {
                Debug.LogError("No IBehaviorTreeSwitcher component found on " + name + "");        
            }
        }

        private void SetTree(IBehaviorNode rootNode) => RootNode = rootNode;

        private void Update()
        {
            // Run all perception modules
            foreach (var perception in Context.PerceptionModules)
                perception.UpdatePerception();   
            
            
            // Persona switcher logic
            if (_personaSwitcher != null && Context != null)
            {
                // TODO - add support for polling(not sure?) but deal eventually deal with the Expensive Invocation
                var newKey = _personaSwitcher.EvaluateSwitch(Context, _activePersonaTreeKey);
                if (!string.IsNullOrEmpty(newKey))
                {
                    SwitchPersonaTree(newKey, "polled switcher");
                }
            }

            // Behavior tree tick
            if (RootNode != null && Context != null)
            {
                var deltaTime = Time.deltaTime;
                var result = RootNode.Tick(Context);
                Context.DeltaTime = deltaTime;
                Context.Blackboard.MovementOrchestrator.Tick(deltaTime);
                Debug.Log($"[BT Tick] Status: {result}");
            }
            
            
        }
    }
}