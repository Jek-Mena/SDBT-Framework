public class SequenceSchema : BtNodeSchema
{
    public SequenceSchema()
    {
        // Composites usually don't have config fields
    }
    public override bool SupportsChildren => true;
}