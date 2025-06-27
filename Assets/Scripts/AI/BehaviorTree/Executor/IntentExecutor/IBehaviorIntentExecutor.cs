public interface IBehaviorIntentExecutor
{
    void ExeccuteIntent(IBehaviorIntentData intent, Blackboard blackboard);
    void CancelIntent();
    bool IsIntentComplete();
}