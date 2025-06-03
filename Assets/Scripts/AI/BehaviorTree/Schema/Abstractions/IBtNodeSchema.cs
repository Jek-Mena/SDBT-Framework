using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public interface IBtNodeSchema
{
    /// <summary>
    /// Retrieves the collection of schema fields defined for the current node schema.
    /// These schema fields describe configuration parameters and their respective properties
    /// such as type, description, default value, and requirements.
    /// </summary>
    /// <returns>
    /// An enumerable collection of <see cref="BtNodeSchemaField"/> objects representing the schema fields.
    /// </returns>
    IEnumerable<BtNodeSchemaField> GetFields();

    /// <summary>
    /// Validates the configuration of a node schema and updates the validation result with any errors or warnings found.
    /// </summary>
    /// <param name="config">The configuration of the node to validate, represented as a JSON object.</param>
    /// <param name="path">The hierarchical path representing the location of the node being validated.</param>
    /// <param name="result">The validation result object used to store errors, warnings, and validity status.</param>
    void Validate(JObject config, string path, BtJsonValidator.ValidationResult result);

    /// <summary>
    /// Gets a value indicating whether the node schema supports having child nodes.
    /// If the value is true, the node is considered a composite or decorator type
    /// and can have child nodes. Otherwise, the node is treated as a leaf node,
    /// which does not support children.
    /// </summary>
    bool SupportsChildren { get; }
}