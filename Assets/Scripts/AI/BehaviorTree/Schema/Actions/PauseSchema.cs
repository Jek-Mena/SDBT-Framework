using Newtonsoft.Json.Linq;

public class PauseSchema : BtNodeSchema
{
    public override bool SupportsChildren => false;
    public PauseSchema()
    {
        AddField(new BtNodeSchemaField
        {
            Key = BtConfigFields.Common.Duration,
            JsonType = JTokenType.Float,
            IsRequired = true,
            AllowRef = true,
            ParamSection = AgentConfigProfileBlocks.Timing
        });
    }
}