using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Editor.BtJson
{
    public class BtFieldRenderDispatcher
    {
        private readonly List<IJsonFieldRenderer> _renderers = new();

        public BtFieldRenderDispatcher(RefSelectorFieldRenderer refRenderer)
        {
            // Ensure RefSelector is always first (for handling $ref)
            _renderers.Add(refRenderer);

            // Register additional renderers (ordered by fallback preference)
            _renderers.Add(new PrimitiveFieldRenderer());
            _renderers.Add(new EnumFieldRenderer());

            // TODO: Add more renderers as needed
            // _renderers.Add(new SliderFieldRenderer());
            // _renderers.Add(new FilePickerFieldRenderer());
        }

        public JToken Render(string key, JToken currentValue, BtNodeSchemaField schemaField)
        {
            foreach (var renderer in _renderers)
            {
                if (renderer.CanRender(schemaField))
                {
                    return renderer.Render(key, currentValue, schemaField);
                }
            }

            EditorGUILayout.LabelField($"[Unknown Renderer] {key}");
            return currentValue;
        }
    }
}