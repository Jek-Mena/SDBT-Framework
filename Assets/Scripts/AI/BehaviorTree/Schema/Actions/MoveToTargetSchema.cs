using Keys;
using Newtonsoft.Json.Linq;

public class MoveToTargetSchema : BtNodeSchema
{
    public override bool SupportsChildren => false;

    public MoveToTargetSchema()
    {
        AddField(new BtNodeSchemaField
        {
            // movement: expects an object (could also be string if your $ref pattern is "movement.profileKey", adjust if needed)
            Key = BtJsonFields.ConfigField,
            JsonType = JTokenType.Object,
            IsRequired = true,
            AllowRef = true,
            RefType = RefSelectorType.Block,
        });
    }
}