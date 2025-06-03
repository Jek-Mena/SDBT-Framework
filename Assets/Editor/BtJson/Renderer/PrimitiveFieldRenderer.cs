using Newtonsoft.Json.Linq;
using UnityEditor;

// A.K.A. Primitive Field Renderer
public class PrimitiveFieldRenderer : IJsonFieldRenderer
{
    public bool CanRender(BtNodeSchemaField field)
    {
        var type = field.JsonType;
        return type == JTokenType.String ||
               type == JTokenType.Integer ||
               type == JTokenType.Float;
    }

    public JToken Render(string key, JToken currentValue, BtNodeSchemaField schemaField)
    {
        EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(key));

        switch (schemaField.JsonType)
        {
            case JTokenType.String:
                return new JValue(EditorGUILayout.TextField(currentValue?.ToString() ?? ""));
            case JTokenType.Integer:
                var intVal = currentValue?.Value<int>() ?? 0;
                return new JValue(EditorGUILayout.IntField(intVal));
            case JTokenType.Float:
                var floatVal = 0f;
                var raw = currentValue?.ToString();

                if (!string.IsNullOrWhiteSpace(raw) && float.TryParse(raw, out var parsedFloat))
                    floatVal = parsedFloat;
                else
                    EditorGUILayout.HelpBox($"Invalid float input for '{key}': '{raw}'", MessageType.Warning);

                var newVal = EditorGUILayout.FloatField(floatVal);
                return new JValue(newVal);
            default:
                EditorGUILayout.LabelField("Unsupported primitive type");
                return currentValue;
        }
    }
}