using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class ParallelSchema : BtNodeSchema
{
    public ParallelSchema()
    {
        // Composites usually don't have config fields'
    }
    
    public override bool SupportsChildren => true;
}