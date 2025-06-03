using Newtonsoft.Json.Linq;

public static class RotationDataBuilder
{
    public static RotationData FromConfig(JObject config, string context)
    {
        JsonUtils.ValidateKeysExist(config,
            BtConfigFields.Rotation.Speed,
            BtConfigFields.Rotation.AngleThreshold,
            BtConfigFields.Rotation.UpdateThreshold);
        
        return new RotationData()
        {
            Speed = JsonUtils.RequireFloat(config, BtConfigFields.Rotation.Speed, context),
            ThresholdAngle = JsonUtils.RequireFloat(config, BtConfigFields.Rotation.AngleThreshold, context),
            UpdateThreshold = JsonUtils.RequireFloat(config, BtConfigFields.Rotation.UpdateThreshold, context)
        };
    }
}