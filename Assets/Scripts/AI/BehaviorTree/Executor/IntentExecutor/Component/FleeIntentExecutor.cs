using UnityEngine;

[System.Obsolete]
public class FleeIntentExecutor : MonoBehaviour, IBehaviorIntentExecutor
{
    private BtContext Context;
    private IMovementNode _movement;
    private FleeIntentData _activeIntent;
    private bool _inProgress = false;
    private float _safeDistance = 10f; // Could be set via intent or config
    
    private const string SciptName = nameof(FleeIntentExecutor);
    
    public void Initialize(BtContext context)
    {
        Context = context;
        _movement = context.Agent.GetComponent<IMovementNode>();
    }
    
    public void ExeccuteIntent(IBehaviorIntentData intent, Blackboard blackboard)
    {
        _activeIntent = intent as FleeIntentData;
        if (_activeIntent == null)
        {
            Debug.LogError($"[{SciptName}] Invalid intent type passed to {nameof(ExeccuteIntent)}!");
            return;
        }

        var fleeDestination = CalculateFleeDestination(_activeIntent);
        if (_movement.TryMoveTo(fleeDestination))
        {
            _inProgress = true;
        }
        else
        {
            Debug.LogError($"[{SciptName}] Failed to move to flee destination!");
        }
    }
    
    public void CancelIntent()
    {
        throw new System.NotImplementedException();
        // Below is ChangeGPT's suggestion.
        // TODO: I am not sure if I should do .StopAgent() when canceling the intent, need further investigation 
        //
        // GPT's suggestion:
        // _movement.StopAgent();
        // _inProgress = false;
        // _activeIntent = null;
    }

    public bool IsIntentComplete()
    {
        throw new System.NotImplementedException();
        // Below is ChangeGPT's suggestion.
        // TODO: I think the IsIntentComplete should also factor TimeoutNode in case destination if far or unreachable, so the agent will still revert back to its default behavior  
        //
        // GPT's suggestion:
        // if (!_inProgress) return true;
        // return _movement.IsAtDestination();
    }
    
    private Vector3 CalculateFleeDestination(FleeIntentData intentData)
    {
        throw new System.NotImplementedException();
        // TODO: I think this should follow the similar structure of the TargetingSystem or the TargetResolver
        // because if I follow ChatGPT's suggestion all the logic for flee will be inside this method.
        //
        // GPT's suggestion:
        // Extend for other flee modes as needed
        
        /*
        if (data.Mode == FleeIntentData.FleeMode.FromSource && data.SourcePosition.HasValue)
        {
            Vector3 direction = (transform.position - data.SourcePosition.Value).normalized;
            return transform.position + direction * _safeDistance;
        }
        else if (data.Mode == FleeIntentData.FleeMode.ToSafePoint && data.TargetPoint.HasValue)
        {
            return data.TargetPoint.Value;
        }
        else
        {
            // Default: random scatter
            return transform.position + Random.onUnitSphere * _safeDistance;
        }
        */
    }
}
