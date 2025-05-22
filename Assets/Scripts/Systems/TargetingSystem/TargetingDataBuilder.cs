using System;
using Newtonsoft.Json.Linq;

public static class TargetingDataBuilder
{
    public static TargetingData FromConfig(JObject config, string context)
    {
        if(config == null)
            throw new ArgumentNullException(nameof(config), $"[{context}] Config cannot be null for TargetingData.");

        var data = new TargetingData
        {
            TargetTag = JsonUtils.RequireString(config, BtConfigFields.Targeting.TargetTag, context),
            Style = Enum.TryParse(
                JsonUtils.RequireString(config, BtConfigFields.Targeting.TargetingStyle, context), 
                out TargetingStyle style) ? style : TargetingStyle.Closest,
            MaxRange = JsonUtils.RequireFloat(config, BtConfigFields.Targeting.MaxRange, context),
            AllowNull = JsonUtils.GetBoolOrDefault(config, BtConfigFields.Targeting.AllowNull, false, context)
        };

        return data;
    }
}