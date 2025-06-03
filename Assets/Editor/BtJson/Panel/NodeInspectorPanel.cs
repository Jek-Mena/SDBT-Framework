using Editor.BtJson.Utilities;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.BtJson.Panel
{
    public class NodeInspectorPanel
    {
        private readonly RefSelectorFieldRenderer _refRenderer;
        private readonly BtFieldRenderDispatcher _dispatcher;

        public NodeInspectorPanel(RefSelectorFieldRenderer renderer)
        {
            _refRenderer = renderer;
            _dispatcher = new BtFieldRenderDispatcher(_refRenderer);
        }

        public void Render(JObject selectedNode)
        {
            using (new VerticalScopeSafe("NodeInspector", null,GUILayout.ExpandWidth(true)))
            {
                EditorGUILayout.LabelField("Node Inspector", EditorStyles.boldLabel);

                if (selectedNode == null)
                {
                    EditorGUILayout.HelpBox("Select a node to inspect", MessageType.Info);
                    return;
                }

                // Type
                EditorGUILayout.LabelField("Type", selectedNode[CoreKeys.Type]?.ToString());

                // Config
                var config = selectedNode[CoreKeys.Config] as JObject;
                if (config == null)
                {
                    EditorGUILayout.HelpBox("No config found.", MessageType.None);
                    return;
                }
                
                if (!BtNodeSchemaRegistry.TryGet(selectedNode[CoreKeys.Type]?.ToString(), out var schema))
                {
                    EditorGUILayout.HelpBox("Schema not found for node type.", MessageType.Warning);
                    return;
                }

                foreach (var field in schema.GetFields())
                {
                    var key = field.Key;
                    var current = config[key];

                    var updated = field.AllowRef
                        ? _refRenderer.Render(key, current, field)
                        : _dispatcher.Render(key, current, field);

                    config[key] = updated;
                }
            }
        }
    }
}