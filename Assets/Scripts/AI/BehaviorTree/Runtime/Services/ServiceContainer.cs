using System.Collections.Generic;
using AI.BehaviorTree.Executor.PhaseUpdate;
using AI.BehaviorTree.Nodes.Actions.Movement;
using AI.BehaviorTree.Nodes.Actions.Rotate;
using AI.BehaviorTree.Nodes.Perception;
using AI.BehaviorTree.Nodes.TemporalControl;
using AI.BehaviorTree.Switching;
using Systems.StatusEffectSystem.Component;
using Systems.TargetingSystem;

namespace AI.BehaviorTree.Runtime.Services
{
    public class ServiceContainer
    {
        // Direct refs, no dictionary
        public StatusEffectManager StatusEffects;
        public TimeExecutionManager TimeExecution;
        public TargetCoordinator Targeting;
        public MovementIntentRouter Movement;
        public RotationIntentRouter Rotation;
        public PersonaBtSwitcher PersonaSwitcher;
        public UpdatePhaseExecutor UpdatePhase;
        public IImpulseNode ImpulseLogic;
        public List<IPerceptionModule> PerceptionModules;
        
        public void Cleanup()
        {
            Movement?.CancelMovement();
            Rotation?.CancelRotation();
            //StatusEffects?.ClearAll();
            //TimeExecution?.ClearAll();
        }
    }
    
    
}