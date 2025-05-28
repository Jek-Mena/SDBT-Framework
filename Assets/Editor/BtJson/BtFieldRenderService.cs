using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;

public class BtFieldRenderService
{
    private readonly List<IJsonFieldRenderer> _renderers = new()
    {
        new PrimitiveFieldRenderer(),
        new EnumFieldRenderer(),
        // You'll register EnumFieldRenderer, RefSelectorRenderer later
        // new FilePickerFieldRenderer(),
        // new SliderFieldRenderer(),
        // new DropdownFieldRenderer()
    };

    public JToken RenderField(string key, JToken currentValue, BtNodeSchemaField schemaField)
    {
        foreach (var renderer in _renderers)
        {
            if (renderer.CanRender(schemaField))
            {
                return renderer.Render(key, currentValue, schemaField);
            }
        }
        
        EditorGUILayout.LabelField($"[Unknown Renderer] {key}");
        
        // If no renderer found, return the current value unchanged
        return currentValue;
    }
}