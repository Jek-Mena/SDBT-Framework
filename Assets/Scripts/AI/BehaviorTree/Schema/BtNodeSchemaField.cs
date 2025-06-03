using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class BtNodeSchemaField
{
    /// <summary>
    /// Represents the unique identifier (AKA label) for a schema field in a behavior tree node.
    /// This property is used to map configuration keys to their respective values
    /// and ensure accurate association with schema definitions.
    /// </summary>
    public string Key { get; set; }

    public JTokenType JsonType { get; set; }

    public JToken DefaultValue { get; set; }
    public string Description { get; set; }
    public string ParamSection { get; set; }

    public float? Min { get; set; }
    public float? Max { get; set; }

    public List<string> EnumValues { get; set; }

    public bool AllowRef { get; set; }

    public RefSelectorType RefType { get; set; } = RefSelectorType.Field;
    
    // Optional: UI hints
    public string UiTypeHint { get; set; } // e.g., "slider", "dropdown", "filepicker"
    public bool IsRequired { get; set; }
}

public enum RefSelectorType
{
    Block, // expects the whole object, e.g., "movement"
    Field  // expects a value, e.g., "movement.speed"
}