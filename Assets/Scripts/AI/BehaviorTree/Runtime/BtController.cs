using System;
using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Loader;
using AI.BehaviorTree.Nodes.Abstractions;
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
        
        private readonly List<ISystemCleanable> _allExitables = new();
        private IBtPersonaSwitcher _personaSwitcher;
        private string _activePersonaTreeKey;

        private string _btSessionId;
        
        // Switches to a new tree by key; this is the only tree assignment API.
        public void SwitchPersonaTree(string treeKey, string reason)
        {
            if (_activePersonaTreeKey == treeKey) return;
        
            if (string.IsNullOrEmpty(treeKey))
            {
                Debug.LogError($"[{ScriptName}] treeKey is null or empty!");
                return;
            }
            
            // Release/Cleanup previous domains before switching
            Context.Blackboard.MovementIntentRouter.ReleaseOwnership(_btSessionId);
            Context.Blackboard.RotationIntentRouter.ReleaseOwnership(_btSessionId);
            ReleaseSystem();
            RootNode?.OnExitNode(Context);
            Debug.Log($"[{ScriptName}] Released ownership of BT '{_activePersonaTreeKey}'");
            
            _activePersonaTreeKey = treeKey;
            Debug.Log($"[{ScriptName}] Switching tree: {_activePersonaTreeKey ?? "(none)"} -> {treeKey} (reason: {reason})");
            
            // Retrieve template (unresolved) from registry
            var btJsonTemplate = BtConfigRegistry.GetTemplate(treeKey);
            if (btJsonTemplate == null)
            {
                Debug.LogError($"[{ScriptName}] Failed to switch—tree key '{treeKey}' not found in registry.");
                return;
            }
            var agentBtJson = btJsonTemplate.DeepClone() as JObject; // Deep clone for isolation
            var rootNode = BtTreeBuilder.LoadTreeFromToken(agentBtJson, Context); // Use current agent's context (must be up to date)

            // Bump the session
            _btSessionId = Guid.NewGuid().ToString();
            Context.Blackboard.BtSessionId = _btSessionId;
            
            // Take ownership for new session
            Context.Blackboard.MovementIntentRouter.TakeOwnership(_btSessionId);
            Context.Blackboard.RotationIntentRouter.TakeOwnership(_btSessionId);
            
            SetTree(rootNode);
            Debug.Log($"[{ScriptName}] Successfully switched to BT '{treeKey}'");
            
            // Assert ownership matches new GUID
            Debug.Assert(Context.Blackboard.MovementIntentRouter.GetActiveOwnerId() == Context.Blackboard.BtSessionId,
                $"[{ScriptName}][ASSERT] Movement domain not owned by current session! Owner={Context.Blackboard.MovementIntentRouter.GetActiveOwnerId()}, Expected={Context.Blackboard.BtSessionId}");
            Debug.Assert(Context.Blackboard.RotationIntentRouter.GetActiveOwnerId() == Context.Blackboard.BtSessionId,
                $"[{ScriptName}][ASSERT] Rotation domain not owned by current session! Owner={Context.Blackboard.RotationIntentRouter.GetActiveOwnerId()}, Expected={Context.Blackboard.BtSessionId}");
        }
    
        public void Initialize(BtContext context)
        {
            Context = context;

            _personaSwitcher = context.Blackboard.PersonaBehaviorTreeSwitcher;
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

        public void RegisterExitable(ISystemCleanable systemCleanable)
        {
            if(!_allExitables.Contains(systemCleanable))
                _allExitables.Add(systemCleanable);
        }
        
        public void ReleaseSystem()
        {
            foreach (var exitable in _allExitables)
            {
                try
                {
                    exitable.ReleaseSystem(Context);
                    Debug.Log($"[{ScriptName}] Exited: {exitable.GetType().Name}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[{ScriptName}] Failed to exit {exitable.GetType().Name}: {ex}");
                }
            }
        }
        
        private void OnSwitchRequested(string fromKey, string toKey, string reason)
        {
            if (toKey != _activePersonaTreeKey)
                SwitchPersonaTree(toKey, $"event: {reason}");
        }
        
        private void SetTree(IBehaviorNode rootNode) => RootNode = rootNode;

        private void Update()
        {
            // Run all perception modules
            foreach (var perception in Context.Blackboard.PerceptionModules)
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
                Context.Blackboard.MovementIntentRouter.Tick(deltaTime);
                Context.Blackboard.RotationIntentRouter.Tick(deltaTime);
                //Debug.Log($"[BT Tick] Status: {result}");
            }
        }
        
        private void LateUpdate()
        {
            if (!BtValidator.Require(Context)
                    .MovementOrchestrator()
                    .Check(out var error))
            {
                Debug.LogError(error);
            }
            
            Context.Blackboard.TimeExecutionManager.LateTick();
            Context.Blackboard.StatusEffectManager.LateTick();
        }
    }
}