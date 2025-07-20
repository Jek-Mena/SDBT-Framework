using AI.BehaviorTree.Core.Data;
using AI.BehaviorTree.Nodes.TemporalControl.Data;
using AI.BehaviorTree.Runtime.Context;
using Utils;

namespace AI.BehaviorTree.Nodes.TemporalControl
{
    public class TimedExecutionComponent
    {
        public TimedExecutionData Data { get; }
        private bool _timerStarted;
        private ITimedExecutionNode _timer;
        
        public TimedExecutionComponent(TimedExecutionData data)
        {
            // Clone the data to prevent cross-node mutation
            Data = new TimedExecutionData()
            {
                Label = data.Label,
                TimerId = data.TimerId,
                Duration = data.Duration,
                Interruptible = data.Interruptible,
                FailOnInterrupt = data.FailOnInterrupt,
                ResetOnExit = data.ResetOnExit,
                Mode = data.Mode
            };
            
            Data.TimerId = Data.Label + this.GetGuid();
        }

        public void Initialize(BtContext context)
        {
            _timer = context.Blackboard.TimeExecutionManager;
            _timerStarted = false;
        }

        public void StartTimerIfNeeded()
        {
            if (_timerStarted) return;
            _timer.StartTime(Data.TimerId, Data.Duration);
            _timerStarted = true;
        }

        public BtStatus GetTimerStatus()
        {
            if (!_timerStarted) return BtStatus.Idle;
            return _timer.IsComplete(Data.TimerId) ? BtStatus.Success : BtStatus.Running; 
        }

        public void InterruptTimer()
        {
            if (!_timerStarted) return;
            _timer.Interrupt(Data.TimerId);
            _timerStarted = false;
        }
    }
}