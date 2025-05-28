using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public interface IBtNodeSchema
{
    void Validate(JObject config, string path, BtJsonValidator.ValidationResult result);

    IEnumerable<BtNodeSchemaField> GetFields();
}