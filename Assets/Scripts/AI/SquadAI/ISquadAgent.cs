using UnityEngine;

namespace AI.SquadAI
{
    public interface ISquadAgent : IGroupBehavior
    {
        Transform Transform { get; }
        void SetSquadSlot(Formation slot, SquadManager manager);
        Vector3 GetSlotWorldPosition();
        bool IsLeader { get; }
    }
    
    public interface IGroupBehavior
    {
        void UpdateFormation();
    }
}