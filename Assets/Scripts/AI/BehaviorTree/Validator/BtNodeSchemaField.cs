using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class BtNodeSchemaField
{
    public string Key { get; set; }
    public JTokenType JsonType { get; set; }

    public JToken DefaultValue { get; set; }
    public string Description { get; set; }
    public string Domain { get; set; }

    public float? Min { get; set; }
    public float? Max { get; set; }

    public List<string> EnumValues { get; set; }

    // Optional: UI hints
    public string UiTypeHint { get; set; } // e.g., "slider", "dropdown", "filepicker"
    public bool IsRequired { get; set; }
}