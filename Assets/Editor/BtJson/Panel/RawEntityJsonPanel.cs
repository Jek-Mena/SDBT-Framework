using System;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public class RawEntityJsonPanel
{
    private bool _isEditing = false;
    private string _rawJson = "";
    private string _error = "";
    private Vector2 _scroll;
    
    private readonly RefSelectorFieldRenderer _refRenderer;

    public RawEntityJsonPanel(RefSelectorFieldRenderer refRenderer)
    {
        _refRenderer = refRenderer;
    }

    public void Render(JObject currentParams, Action<JObject> onParamsUpdated)
    {
        EditorGUILayout.LabelField("Raw Entity Config JSON", EditorStyles.boldLabel);

        if (!_isEditing)
        {
            // View mode
            if (!GUILayout.Button("Edit")) return;
            _isEditing = true;
            _rawJson = currentParams?.ToString(Newtonsoft.Json.Formatting.Indented) ?? "";
            _error = "";
        }
        else
        {
            // Edit mode
            if (GUILayout.Button("Apply"))
            {
                try
                {
                    // Try parsed edited JSON
                    var parsed = JObject.Parse(_rawJson);
                    onParamsUpdated(parsed);;
                    _refRenderer.SetParamRoot(parsed);
                    _isEditing = false;
                    _error = "";
                }
                catch (Exception e)
                {
                    _error = $"Error parsing JSON: {e.Message}";
                }
            }
            
            if (GUILayout.Button("Cancel"))
            {
                _isEditing = false;
                _error = "";
            }
        }

        _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(300));
        _rawJson = _isEditing 
            ? EditorGUILayout.TextArea(_rawJson, GUILayout.ExpandHeight(true))
            : currentParams?.ToString(Newtonsoft.Json.Formatting.Indented) ?? "";
        EditorGUILayout.EndScrollView();
        
        if (!string.IsNullOrEmpty(_error))
            EditorGUILayout.HelpBox(_error, MessageType.Error);
    }
}