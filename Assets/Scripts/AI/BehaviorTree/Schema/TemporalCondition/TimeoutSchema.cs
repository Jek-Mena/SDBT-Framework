using Newtonsoft.Json.Linq;

public class TimeoutSchema : BtNodeSchema
{
    public TimeoutSchema()
    {
        AddField(new BtNodeSchemaField
        {
            Key = BtConfigFields.Common.Label,
            JsonType = JTokenType.String,
            IsRequired = true,
            AllowRef = false,
            ParamSection = CoreKeys.ParamSections.Timing
        });
        AddField(new BtNodeSchemaField
        {
            Key = BtConfigFields.Common.Duration,
            JsonType = JTokenType.Float,
            IsRequired = true,
            AllowRef = true,
            ParamSection = CoreKeys.ParamSections.Timing
        });
    }

    public override bool SupportsChildren => false;
}