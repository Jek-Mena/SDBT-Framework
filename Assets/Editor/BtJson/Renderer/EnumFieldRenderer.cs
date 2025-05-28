using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public class EnumFieldRenderer : IJsonFieldRenderer
{
    public bool CanRender(BtNodeSchemaField field)
    {
        return field.EnumValues is { Count: > 0 };
    }

    public JToken Render(string key, JToken currentValue, BtNodeSchemaField field)
    {
        var selectedIndex = 0;

        string[] options = field.EnumValues.ToArray();
        var current = currentValue?.ToString() ?? options[0];

        for (int i = 0; i < options.Length; i++)
        {
            if (options[i] == current)
            {
                selectedIndex = i;
                break;
            }
        }
        
        var newIndex = EditorGUILayout.Popup(new GUIContent(key, field.Description), selectedIndex, options);
        return JToken.FromObject(options[newIndex]);
    }
}
