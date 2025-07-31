using System;
using System.Collections.Generic;
using System.Linq;
using AI.BehaviorTree.Core;
using AI.BehaviorTree.Loader;
using AI.BehaviorTree.Nodes.Abstractions;
using AI.BehaviorTree.Registry;
using AI.BehaviorTree.Runtime.Context;
using AI.BehaviorTree.Switching;
using AI.GroupAI.SquadAgent;
using Newtonsoft.Json.Linq;
using Systems.Abstractions;
using UnityEngine;

namespace AI.BehaviorTree.Runtime
{
    public class BtController : MonoBehaviour, IAgentRuntimeData
    {
        private const string ScriptName = nameof(BtController);
        
        public BtContext Context;
        public IBehaviorNode RootNode { get; private set; }
        
        private readonly List<ISystemUpdatable> _allUpdatables = new();
        private readonly List<ISystemCleanable> _allExitables = new();
        private PersonaBtSwitcher _personaSwitcher;
        public string ActivePersonaTreeKey { get; private set; }
        private string _btSessionId;
        
        // Switches to a new tree by key; this is the only tree assignment API.
        public void SwitchPersonaTree(string treeKey, string reason)
        {
            if (ActivePersonaTreeKey == treeKey) return;
            if (string.IsNullOrEmpty(treeKey))
            {
                Debug.LogError($"[{ScriptName}] treeKey is null or empty!");
                return;
            }
            
            // ---[1. Release/cleanup all possible system state before switching]---
            ReleaseAllSystem();
            
            // ---[2. Validate no status/effects have leaked before proceeding]---
            var effects = Context.Blackboard.StatusEffectManager.GetActiveEffects().ToList();
            if (effects.Count > 0)
            {
                Debug.LogError($"[{ScriptName}] After OnExitNode, leaked {effects.Count} effects:");
                foreach (var effect in effects)
                    Debug.LogError($"Leaked effect: {effect.Name}, Domains: {string.Join(",", effect.Domains)}");
            }
            
            ActivePersonaTreeKey = treeKey;
            Debug.Log($"[{ScriptName}] Switching tree: {ActivePersonaTreeKey ?? "(none)"} -> {treeKey} (reason: {reason})");
            
            // ---[3. Build new tree from registry and assign]---
            var btJsonTemplate = BtConfigRegistry.RequireTemplate(treeKey);

            var agentBtJson = btJsonTemplate.DeepClone() as JObject; // Deep clone for isolation
            var rootNode = BtTreeBuilder.LoadTreeFromToken(agentBtJson, Context); // Use current agent's context (must be up to date)

            // ---[4. Bump BT session, take ownership, and assign root]---
            _btSessionId = Guid.NewGuid().ToString();
            Context.Blackboard.BtSessionId = _btSessionId;
            Context.Blackboard.MovementIntentRouter.TakeOwnership(_btSessionId);
            Context.Blackboard.RotationIntentRouter.TakeOwnership(_btSessionId);
            
            RootNode = rootNode;

            Debug.Log($"[{ScriptName}] Successfully switched to BT '{treeKey}'");
            
            Debug.Assert(Context.Blackboard.MovementIntentRouter.GetActiveOwnerId() == Context.Blackboard.BtSessionId,
                $"[{ScriptName}][ASSERT] Movement domain not owned by current session! Owner={Context.Blackboard.MovementIntentRouter.GetActiveOwnerId()}, Expected={Context.Blackboard.BtSessionId}");
            Debug.Assert(Context.Blackboard.RotationIntentRouter.GetActiveOwnerId() == Context.Blackboard.BtSessionId,
                $"[{ScriptName}][ASSERT] Rotation domain not owned by current session! Owner={Context.Blackboard.RotationIntentRouter.GetActiveOwnerId()}, Expected={Context.Blackboard.BtSessionId}");
            
            Debug.Assert(effects.Count == 0, $"[BT] State leak: {effects.Count} status effects active after BT/persona switch!");
        }
    
        public void Initialize(BtContext context)
        {
            Context = context;

            _personaSwitcher = context.Blackboard.PersonaBtSwitcher;
            if (_personaSwitcher != null)
                _personaSwitcher.OnSwitchRequested += OnSwitchRequested;
            else
                Debug.LogError("No IBehaviorTreeSwitcher component found on " + name);

            if (_personaSwitcher != null)
            {
                Debug.Log($"[{ScriptName}] Start() called on {gameObject.name}. Context: {Context != null}. PersonaSwitcher: {_personaSwitcher != null}");
                var initialPersonaKey = _personaSwitcher.EvaluateSwitch(Context, ActivePersonaTreeKey);
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

        public void RegisterUpdatable(ISystemUpdatable systemUpdatable)
        {
            if(!_allUpdatables.Contains(systemUpdatable))
                _allUpdatables.Add(systemUpdatable);       
        }
        
        private void ReleaseAllSystem()
        {
            // Call OnExitNode on the old root recursively, clearing ALL status/timers/blocks from BT nodes
            // This also includes the intent routers 
            RootNode?.OnExitNode(Context);
            RootNode = null; // Defensive: ensures nothing references the dead tree
            
            foreach (var exitable in _allExitables)
            {
                try
                {
                    exitable.ReleaseSystem(Context);
                    //Debug.Log($"[{ScriptName}] Exited: {exitable.GetType().Name}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[{ScriptName}] Failed to exit {exitable.GetType().Name}: {ex}");
                }
            }
        }
        
        private void OnSwitchRequested(string fromKey, string toKey, string reason)
        {
            if (toKey != ActivePersonaTreeKey)
                SwitchPersonaTree(toKey, $"event: {reason}");
        }

        private void Update()
        {
            // Run all perception modules
            foreach (var perception in Context.Blackboard.PerceptionModules)
                perception.UpdatePerception();   
            
            // Persona switcher logic
            if (_personaSwitcher != null && Context != null)
            {
                // TODO - add support for polling(not sure?) but deal eventually deal with the Expensive Invocation
                var newKey = _personaSwitcher.EvaluateSwitch(Context, ActivePersonaTreeKey);
                if (!string.IsNullOrEmpty(newKey))
                {
                    SwitchPersonaTree(newKey, "polled switcher");
                }
            }

            foreach (var systemUpdatable in _allUpdatables)
                systemUpdatable.Update(Context);
            
            foreach (var groupBehavior in Context.Blackboard.GetAll<IGroupBehavior>())
                groupBehavior.UpdateFormation();
            
            // Behavior tree tick
            if (RootNode != null && Context != null)
            {
                Context.DeltaTime = Time.deltaTime;
                var result = RootNode.Tick(Context);
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
        
        public Transform Transform => transform;
        private void OnEnable()  => AgentRuntimeRegistry.Register(this);
        private void OnDisable() => AgentRuntimeRegistry.Unregister(this);
    }
}