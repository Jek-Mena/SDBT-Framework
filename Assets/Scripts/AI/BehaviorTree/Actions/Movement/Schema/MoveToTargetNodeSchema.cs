using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class MoveToTargetNodeSchema : IBtNodeSchema
{
    private static readonly JTokenType[] NumberTypes = new[] { JTokenType.Float, JTokenType.Integer };
    
    public void Validate(JObject config, string path, BtJsonValidator.ValidationResult result)
    {
        BtSchemaUtils.ValidateField(config, BtConfigFields.Movement.Speed, NumberTypes, path, result);
        BtSchemaUtils.ValidateField(config, BtConfigFields.Movement.Acceleration, NumberTypes, path, result);
        BtSchemaUtils.ValidateField(config, BtConfigFields.Movement.AngularSpeed, NumberTypes, path, result);
        BtSchemaUtils.ValidateField(config, BtConfigFields.Movement.StoppingDistance, NumberTypes, path, result);
        BtSchemaUtils.ValidateField(config, BtConfigFields.Movement.UpdateThreshold, NumberTypes, path, result);
    }

    public IEnumerable<BtNodeSchemaField> GetFields()
    {
        return new[]
        {
            new BtNodeSchemaField
            {
                Key = BtConfigFields.Movement.Speed,
                JsonType = JTokenType.Float,
                Description = "Movement speed",
                IsRequired = false,
                DefaultValue = JToken.FromObject(3f),
                Domain = DomainKeys.Movement
            },
            new BtNodeSchemaField
            {
                Key = BtConfigFields.Movement.Acceleration,
                JsonType = JTokenType.Float,
                Description = "Movement acceleration",
                IsRequired = false,
                DefaultValue = JToken.FromObject(3f),
                Domain = DomainKeys.Movement
            },
            new BtNodeSchemaField
            {
                Key = BtConfigFields.Movement.AngularSpeed,
                JsonType = JTokenType.Float,
                Description = "Angular turning speed",
                IsRequired = false,
                DefaultValue = JToken.FromObject(120f),
                Domain = DomainKeys.Movement
            },
            new BtNodeSchemaField
            {
                Key = BtConfigFields.Movement.StoppingDistance,
                JsonType = JTokenType.Float,
                Description = "Stopping distance",
                IsRequired = false,
                DefaultValue = JToken.FromObject(1f),
                Domain = DomainKeys.Movement
            },
            new BtNodeSchemaField
            {
                Key = BtConfigFields.Movement.UpdateThreshold,
                JsonType = JTokenType.Float,
                Description = "Min distance to re-path",
                IsRequired = false,
                DefaultValue = JToken.FromObject(0.5f),
                Domain = DomainKeys.Movement
            },

        };
    }
}