using System.Collections.Generic;
using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Runtime.Context;

namespace AI.BehaviorTree.Nodes.Abstractions
{
    public interface IBehaviorNode
    {
        public void Initialize(BtContext context);
        BtStatus Tick(BtContext context); // Intent in -> Tick -> Status out.
        void Reset(BtContext context);
        void OnExitNode(BtContext context);
        IEnumerable<IBehaviorNode> GetChildren { get; } // Expose children if any
        
        // For display/debug
        BtStatus LastStatus { get; }
        string DisplayName { get;  }
    }
}