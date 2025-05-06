using System;
using Newtonsoft.Json.Linq;

public interface IBtNodeFactory
{
    IBehaviorNode CreateNode(JObject config, Blackboard blackboard, Func<JToken, IBehaviorNode> buildChild);
    // void ValidateConfig(JObject config); // Optional
}