using System.Collections.Generic;
using UnityEngine;

public class UpdatePhaseExecutor : MonoBehaviour
{
    private readonly Queue<IGameAction> _updateQueue = new();
    private readonly Queue<IGameAction> _lateUpdateQueue = new();
    private readonly Queue<IGameAction> _fixedUpdateQueue = new();

    public void EnqueueUpdate(IGameAction action) => _updateQueue.Enqueue(action);
    public void EnqueueLateUpdate(IGameAction action) => _lateUpdateQueue.Enqueue(action);
    public void EnqueueFixedUpdate(IGameAction action) => _fixedUpdateQueue.Enqueue(action);

    private void Update()
    {
        while (_updateQueue.Count > 0)
        {
            var action = _updateQueue.Dequeue();
            action.Execute();
        }
    }

    private void LateUpdate()
    {
        while (_lateUpdateQueue.Count > 0)
        {
            var action = _lateUpdateQueue.Dequeue();
            action.Execute();
        }
    }

    private void FixedUpdate()
    {
        while (_fixedUpdateQueue.Count > 0)
        {
            var action = _fixedUpdateQueue.Dequeue();
            action.Execute();
        }
    }
}

/*
using UnityEngine;
   
   public class DashCommand : IGameCommand
   {
       private readonly Vector3 _direction;
       private readonly float _speed;
   
       public DashCommand(Vector3 direction, float impulseStrength)
       {
           _direction = direction;
           _speed = impulseStrength;
       }
   
       public void Execute(GameObject context)
       {
           if (context.TryGetComponent<Rigidbody>(out var rb))
           {
               rb.MovePosition(rb.position + _direction.normalized * _speed * Time.fixedDeltaTime);
           }
       }
   }
   

public class PlayAnimationCommand : IGameCommand
   {
       private readonly string _trigger;
   
       public PlayAnimationCommand(string trigger)
       {
           _trigger = trigger;
       }
   
       public void Execute(GameObject context)
       {
           if (context.TryGetComponent<Animator>(out var animator))
           {
               animator.SetTrigger(_trigger);
           }
       }
   }

public class EnemyAI : MonoBehaviour
   {
       private TimedCommandExecutor _executor;
   
       private void Awake()
       {
           _executor = GetComponent<TimedCommandExecutor>();
       }
   
       public void DashAtPlayer(Vector3 playerPosition)
       {
           Vector3 direction = (playerPosition - transform.position).normalized;
           _executor.EnqueueCommand(new DashCommand(direction, 10f));
           _executor.EnqueueCommand(new PlayAnimationCommand("ImpulseLogic"));
       }
   }
   
public class DelayedCommand : IGameCommand
   {
       private readonly float _delay;
       private readonly IGameCommand _inner;
       private float _elapsed;
   
       public DelayedCommand(float delay, IGameCommand inner)
       {
           _delay = delay;
           _inner = inner;
       }
   
       public void Execute(GameObject context)
       {
           context.GetComponent<MonoBehaviour>().StartCoroutine(ExecuteAfterDelay(context));
       }
   
       private IEnumerator ExecuteAfterDelay(GameObject context)
       {
           yield return new WaitForSeconds(_delay);
           _inner.Execute(context);
       }
   }
   
*/
