using UnityEngine;

public interface IImpulseNode
{
    bool TryImpulse(); // Queues it
    void PerformImpulse(Vector3 direction); // Used by command (IGameAction)
    bool IsImpulsing();
    bool IsImpulseComplete();
}