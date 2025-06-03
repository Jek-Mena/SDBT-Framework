using Newtonsoft.Json.Linq;

public interface IJsonFieldRenderer
{
    bool CanRender(BtNodeSchemaField field);

    // Draws the field and returns the edited value.
    JToken Render(string key, JToken currentValue, BtNodeSchemaField schemaField);
}