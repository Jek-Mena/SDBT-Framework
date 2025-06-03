using Newtonsoft.Json.Linq;

public class RotationActionBuilder
{
    /// <summary>
    /// Constructs a JSON object representing a rotation target with predefined structure and settings.
    /// The name 'RotateToTarget' is similar to BtNodeTypes.Rotation.RotateToTarget <see cref="BtNodeTypes.Rotation.RotateToTarget"/>, but it is not a behavior node.
    /// </summary>
    /// <param name="rotationKey">
    /// The key representing the rotation configuration. Defaults to CoreKeys.SettingsBlock.Rotation if not provided.
    /// </param>
    /// <returns>
    /// A JSON object containing the type and configuration fields for the rotation target.
    /// </returns>
    public JObject RotateToTarget(string rotationKey = CoreKeys.ParamSections.Rotation) =>
        new JObject()
        {
            [CoreKeys.Type] = BtNodeTypes.Rotation.RotateToTarget,
            [CoreKeys.Config] = new JObject { [CoreKeys.Ref] = rotationKey }
        };
}