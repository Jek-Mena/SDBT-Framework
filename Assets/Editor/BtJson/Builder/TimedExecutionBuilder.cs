using Newtonsoft.Json.Linq;

public class TimedExecutionBuilder
{
    public JObject Pause(TimedExecutionData data, string domain = DomainKeys.Default)
    {
        return new JObject
        {
            [CoreKeys.Type] = BtNodeTypes.TimedExecution.Pause,
            [CoreKeys.Config] = new JObject
            {
                [BtConfigFields.Common.Label] = data.Label,
                [BtConfigFields.Common.Duration] = data.Duration,
                [BtConfigFields.Common.Domains] = new JObject
                {
                    [CoreKeys.Ref] = domain
                } 
            }
        };
    }
}