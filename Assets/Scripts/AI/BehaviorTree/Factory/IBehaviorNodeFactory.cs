using Newtonsoft.Json.Linq;

public interface IBehaviorNodeFactory
{
    IBehaviorNode CreateNode(JObject config, Blackboard blackboard);
}