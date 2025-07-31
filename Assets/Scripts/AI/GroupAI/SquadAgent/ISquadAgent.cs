using UnityEngine;

namespace AI.GroupAI.SquadAgent
{
    public interface ISquadAgent : IGroupBehavior
    {
        public Formation Formation { get; set; }
        SquadManager SquadManager { get; set; }
        Transform Transform { get; }
        
        bool IsLeader { get; }
        
        void SetSquadSlot(Formation slot, SquadManager manager);
        
        Vector3 GetSlotWorldPosition();
    }
}