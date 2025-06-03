using Newtonsoft.Json.Linq;

public static class MovementDataBuilder
{
    public static MovementData FromConfig(JObject config, string context)
    {
        JsonUtils.ValidateKeysExist(config,
            BtConfigFields.Movement.Speed,
            BtConfigFields.Movement.Acceleration,
            BtConfigFields.Movement.AngularSpeed,
            BtConfigFields.Movement.StoppingDistance,
            BtConfigFields.Movement.UpdateThreshold);

        return new MovementData()
        {
            Speed = JsonUtils.RequireFloat(config, BtConfigFields.Movement.Speed, context),
            Acceleration = JsonUtils.RequireFloat(config, BtConfigFields.Movement.Acceleration, context),
            AngularSpeed = JsonUtils.RequireFloat(config, BtConfigFields.Movement.AngularSpeed, context),
            StoppingDistance = JsonUtils.RequireFloat(config, BtConfigFields.Movement.StoppingDistance, context),
            UpdateThreshold = JsonUtils.RequireFloat(config, BtConfigFields.Movement.UpdateThreshold, context)
        };
    }
}