using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.BtJson.Panel
{
    public class RawBtJsonPanel
    {
        private bool _isVisible = true;
        private Vector2 _scroll;
        
        public void Render(JObject treeJson)
        {
            if(treeJson == null) return;
            
            if (GUILayout.Button(_isVisible ? "Hide Raw BT JSON" : "Show Raw BT JSON", EditorStyles.toolbarButton))
                _isVisible = !_isVisible;
                
            if (!_isVisible) return;

            EditorGUILayout.LabelField("Raw BT JSON", EditorStyles.boldLabel);
        
            
            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(300));
            EditorGUILayout.TextArea(treeJson.ToString(Newtonsoft.Json.Formatting.Indented), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }
}