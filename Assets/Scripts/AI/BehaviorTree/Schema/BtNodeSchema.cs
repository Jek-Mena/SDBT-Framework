using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public abstract class BtNodeSchema: IBtNodeSchema
{
    private readonly List<BtNodeSchemaField> _fields = new();
    public abstract bool SupportsChildren { get; }
    public IEnumerable<BtNodeSchemaField> GetFields() => _fields;

    protected BtNodeSchema AddField(BtNodeSchemaField field)
    {
        _fields.Add(field);
        return this;
    }
    
    public virtual void Validate(JObject config, string path, BtJsonValidator.ValidationResult result)
    {
        if (config == null)
        {
            result.Errors.Add($"[{path}] Missing or invalid config object.");
            return;
        }
        
        foreach (var field in _fields)
        {
            if (!config.TryGetValue(field.Key, out var token) && field.IsRequired)
            {
                result.Errors.Add($"[{path}].config Missing required field key: '{field.Key}'");
                continue;
            }

            if (token.Type == JTokenType.Object && token[BtJsonFields.Ref] != null)
            {
                if(!field.AllowRef)
                    result.Errors.Add($"[{path}].config Field '{field.Key}' does not support referencing other nodes.");
                
                continue;
            }
            
            if (token.Type != field.JsonType)
                result.Errors.Add($"[{path}].config Field '{field.Key}' has invalid type. Expected: {field.JsonType}, Actual: {token.Type}");
        }
    }
}
