using Newtonsoft.Json.Linq;

public static class RotationDataBuilder
{
    public static RotationData FromConfig(JObject config, string context)
    {
        JsonUtils.ValidateKeysExist(config,
            EntityJsonFields.BehaviorProfiles.RotationProfile.Speed,
            EntityJsonFields.BehaviorProfiles.RotationProfile.AngleThreshold,
            EntityJsonFields.BehaviorProfiles.RotationProfile.UpdateThreshold);
        
        return new RotationData()
        {
            Speed = JsonUtils.RequireFloat(config, EntityJsonFields.BehaviorProfiles.RotationProfile.Speed, context),
            ThresholdAngle = JsonUtils.RequireFloat(config, EntityJsonFields.BehaviorProfiles.RotationProfile.AngleThreshold, context),
            UpdateThreshold = JsonUtils.RequireFloat(config, EntityJsonFields.BehaviorProfiles.RotationProfile.UpdateThreshold, context)
        };
    }
}