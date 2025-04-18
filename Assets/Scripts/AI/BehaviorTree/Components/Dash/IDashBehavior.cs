using UnityEngine;

public interface IDashBehavior
{
    bool TryDashTo(Vector3 destination); // Queues it
    void PerformDash(Vector3 destination); // Used by command
    bool IsDashing();
    bool IsDashComplete();
}