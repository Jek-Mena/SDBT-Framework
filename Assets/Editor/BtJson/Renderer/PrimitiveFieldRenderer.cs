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
                var floatVal = currentValue?.Value<float>() ?? 0; 
                return new JValue(EditorGUILayout.FloatField(floatVal));
            default:
                EditorGUILayout.LabelField("Unsupported primitive type");
                return currentValue;
        }
    }
}