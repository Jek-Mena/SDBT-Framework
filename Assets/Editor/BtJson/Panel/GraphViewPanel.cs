using System;
using Keys;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.BtJson.Panel
{
    public class GraphViewPanel
    {
        private JObject _tree;
        private JObject _selectedNode;
        public Action<JObject> OnNodeSelected;

        public GraphViewPanel(JObject treeJson)
        {
            _tree = treeJson;
        }

        public void SetTree(JObject treeJson)
        {
            _tree = treeJson;
        }
        
        public void Render()
        {
            EditorGUILayout.BeginScrollView(Vector2.zero);
            try
            {
                var root = _tree[BtJsonFields.Root] as JObject;
                if (root != null)
                    DrawNodeTree(root, 0, null, -1);
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }
        }
    
        private void DrawNodeTree(JObject node, int indent, JArray parentArray, int indexInParent)
        {
            var shouldDelete = false;
        
            EditorGUILayout.BeginHorizontal();
        
            try
            {
                GUILayout.Space(indent * 20f);
        
                var nodeType = node[BtJsonFields.Type]?.ToString() ?? "(unknown)";
                var isSelected = node == _selectedNode;
                var label = isSelected ? $"▶ {nodeType}" : nodeType;
            
                if (GUILayout.Button(label, EditorStyles.miniButton))
                {
                    _selectedNode = node;
                    OnNodeSelected?.Invoke(node); // Notify parent (BtEditorWindow) that a node was selected.
                }
            
                if (parentArray != null && GUILayout.Button("X", GUILayout.Width(20f)))
                {
                    shouldDelete = true; 
                    GUI.FocusControl(null);
                }
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }

            if (shouldDelete)
            {
                parentArray.RemoveAt(indexInParent);
                return; // exit early to avoid drawing deleted node
            }
        
            // Draw Children (if any)
            var children = node[BtJsonFields.Children] as JArray;
            if (children != null)
            {
                for (var i = 0; i < children.Count; i++)
                {
                    if (children[i] is JObject childNode)
                        DrawNodeTree(childNode, indent + 1, children, i);
                }

                // Add Child dropdown button
                EditorGUILayout.BeginHorizontal();
                try
                {
                    GUILayout.Space((indent + 1) * 20f);

                    if (GUILayout.Button("+ Add Child", EditorStyles.miniButton))
                    {
                        var menu = new GenericMenu();
                        foreach (var nodeType in BtNodeSchemaRegistry.GetAllNodeTypes)
                        {
                            menu.AddItem(new GUIContent(nodeType), false, () =>
                            {
                                var newChild = BtEditorNodeFactory.CreateNewNode(nodeType);
                                children.Add(newChild);
                            });
                        }
                        menu.ShowAsContext();
                    }
                }
                finally
                {
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}