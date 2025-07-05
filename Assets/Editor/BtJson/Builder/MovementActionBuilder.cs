using AI.BehaviorTree.Keys;
using Newtonsoft.Json.Linq;

public class MovementActionBuilder
{
    /// <summary>
    /// Constructs a JSON object representing a movement target with predefined structure and settings.
    /// The name 'MoveToTarget' is similar to BtNodeTypes.Movement.MoveToTarget <see cref="BtNodeTypes.Movement.MoveToTarget"/>, but it is not a behavior node.
    /// </summary>
    /// <param name="movementKey">
    /// The key representing the movement configuration. Defaults to CoreKeys.SettingsBlock.Movement if not provided.
    /// </param>
    /// <returns>
    /// A JSON object containing the type and configuration fields for the movement target.
    /// </returns>
    public JObject MoveToTarget(string movementKey = BtNodeProfileSelectorKeys.Movement)
    {
        return new JObject
        {
            [CoreKeys.Type] = BtNodeTypes.Movement.MoveToTarget,
            [CoreKeys.Config] = new JObject
            {
                [CoreKeys.Ref] = movementKey
            }
        };
    }
}