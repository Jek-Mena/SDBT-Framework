using System.Collections.Generic;
using AI.BehaviorTree.Core;
using AI.BehaviorTree.Nodes.Actions.Movement;
using AI.BehaviorTree.Nodes.Actions.Movement.Components;
using AI.BehaviorTree.Nodes.Actions.Rotate;
using AI.BehaviorTree.Switching;
using UnityEngine;

namespace AI.BehaviorTree.Runtime.Context
{
    /// <summary>
    /// [2025-06-18] Only construct via the builder pipeline. Never new up outside context build!
    /// Represents the context for a behavior tree execution in an AI system.
    /// This context encapsulates references to the AI entity itself, its blackboard for shared data,
    /// and the behavior tree controller managing the execution flow.
    ///
    /// [ARCHITECTURAL RULE]
    /// There is one BtContext per entity. It is constructed only by the context builder pipeline.
    /// All systems must use the canonical instance set on BtController.Context.
    /// Never new up a context in plugins, nodes, or runtime.
    /// </summary>
    public class BtContext
    {
        public BtController Controller { get; }
        public GameObject Agent { get; }
        public AgentProfiles AgentProfiles { get; }
        public AgentDefinition Definition { get; }
        public Blackboard Blackboard { get; }
        public BtPersonaSwitcher PersonaSwitcher { get; }
        public List<IPerceptionModule> PerceptionModules { get; set; }

        public float DeltaTime;
        
        public BtContext(
            GameObject agent,
            BtController controller,
            AgentProfiles profile, 
            AgentDefinition definition,
            Blackboard blackboard,
            BtPersonaSwitcher personaSwitcher)
        {
            Controller = controller;
            Agent = agent;
            AgentProfiles = profile;
            Definition = definition;
            Blackboard = blackboard;
            PersonaSwitcher = personaSwitcher;
        }
    }
}