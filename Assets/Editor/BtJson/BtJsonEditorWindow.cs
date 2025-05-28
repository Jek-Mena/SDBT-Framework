using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;


// TODO Optional Fields (not rendered unless present) — Future Option
// Choose later whether to:
// • Render optional fields if present
// • Or add a toggle to inject them via "Add Optional Field"
//
// TODO: Grouping and Ordering 
//  .Group and sort fields later to display logical blocks — nice for large config editors.
public class BtJsonEditorWindow : EditorWindow
{
    private string _currentFilePath;
    private JObject _jsonRoot;
    private Vector2 _scroll;
    private string _validationLog;

    private static readonly Vector2 MinWindowSize = new Vector2(600, 400);
    
    private readonly Dictionary<string, bool> _groupFoldouts = new();
    private readonly BtFieldRenderService _fieldRenderService = new BtFieldRenderService();
    
    [MenuItem("Tools/Behavior Tree/JSON Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<BtJsonEditorWindow>();
        window.titleContent = new GUIContent("BT JSON Editor");
        window.minSize = MinWindowSize;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Open JSON", EditorStyles.toolbarButton))
            OpenJson();

        if (!string.IsNullOrEmpty(_currentFilePath) && GUILayout.Button("Save", EditorStyles.toolbarButton))
            SaveJson();

        if (_jsonRoot != null && GUILayout.Button("Validate", EditorStyles.toolbarButton))
            RunValidation();
        EditorGUILayout.EndHorizontal();

        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        try
        {
            if (_jsonRoot == null)
                EditorGUILayout.HelpBox("Open a JSON file to begin.", MessageType.Info);
            else
            {
                EditorGUILayout.LabelField("Behavior Tree Structure", EditorStyles.boldLabel);

                if (_jsonRoot.TryGetValue(CoreKeys.Root, out var rootNodeToken) && rootNodeToken is JObject rootNode)
                    DrawNode(new JProperty(CoreKeys.Root, rootNode));
                else
                    EditorGUILayout.HelpBox($"Invalid or missing '{CoreKeys.Root}' node in JSON", MessageType.Error);

                if (!string.IsNullOrEmpty(_validationLog))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox(_validationLog, MessageType.Warning);
                }
            }
        }
        finally
        {
            EditorGUILayout.EndScrollView();
        }
    }


    private void OpenJson()
    {
        var path = EditorUtility.OpenFilePanel("Open BT JSON", "Assets/Resources/Data", "json");
        if (string.IsNullOrEmpty(path)) return;

        var text = File.ReadAllText(path);
        try
        {
            _jsonRoot = JObject.Parse(text);
            _currentFilePath = path;
            _validationLog = null;
        }
        catch (Exception e)
        {
            _validationLog = e.Message;
        }
    }

    private void SaveJson()
    {
        if (string.IsNullOrEmpty(_currentFilePath)) return;

        var json = _jsonRoot.ToString(Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(_currentFilePath, json); 
        AssetDatabase.Refresh();
    }

    private void RunValidation()
    {
        BtNodeSchemaRegistrationList.InitializeDefaults();
        var result = BtJsonValidator.ValidateFromJObject(_jsonRoot);

        _validationLog = result.IsValid ? "VALID" : string.Join("\n", result.Errors);
    }

    private void DrawConfigField(string key, JObject config, BtNodeSchemaField field)
    {
        config.TryGetValue(key, out var value);

        EditorGUILayout.BeginVertical(GUI.skin.GetStyle("Box"));
        try
        {
            var updatedValue = _fieldRenderService.RenderField(key, value, field);
            if (!JToken.DeepEquals(updatedValue, value))
            {
                config[key] = updatedValue;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Field render exception for '{key}': {e}");
            EditorGUILayout.HelpBox($"[Error] Failed to render field: {key}", MessageType.Error);
        }
        finally
        {
            EditorGUILayout.EndVertical();
        }
    }


    private void DrawNode(JProperty nodeProperty, string path = CoreKeys.Root)
    {
        if (nodeProperty.Value is not JObject nodeObject) return;

        EditorGUILayout.BeginVertical(GUI.skin.GetStyle("Box"));
        try
        {
            EditorGUILayout.LabelField($"Node: {nodeProperty.Name}", EditorStyles.boldLabel);

            var type = nodeObject[CoreKeys.Type]?.ToString() ?? "???";

            if (nodeObject[CoreKeys.Config] is JObject config)
            {
                EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);

                if (BtNodeSchemaRegistry.TryGet(type, out var schema))
                {
                    string currentGroup = null;

                    foreach (var field in schema.GetFields())
                    {
                        var key = field.Key;
                        var tooltip = field.Description;
                        var group = field.Domain ?? "Default";

                        if (group != currentGroup)
                        {
                            currentGroup = group;
                            _groupFoldouts.TryAdd(group, true);
                            _groupFoldouts[group] = EditorGUILayout.Foldout(_groupFoldouts[group], group, true);
                        }

                        if (!_groupFoldouts[group])
                            continue;

                        var label = new GUIContent(key, tooltip);
                        var hasValue = config.TryGetValue(key, out var value);

                        // Optional field toggles
                        if (!field.IsRequired && !hasValue)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(label);

                            if (GUILayout.Button("+ Add", GUILayout.Width(60)))
                                config[key] = field.DefaultValue ?? JToken.FromObject(BtSchemaUtils.GetDefaultForType(field.JsonType));

                            EditorGUILayout.EndHorizontal();
                            continue;
                        }

                        if (!hasValue)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox($"Missing required field: {key}", MessageType.Warning);

                            if (GUILayout.Button("Fix", GUILayout.Width(40)))
                                config[key] = field.DefaultValue ?? JToken.FromObject(BtSchemaUtils.GetDefaultForType(field.JsonType));

                            EditorGUILayout.EndHorizontal();
                            continue;
                        }

                        DrawConfigField(key, config, field);

                        // Optional field: Allow removal
                        if (!field.IsRequired)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("X Remove", GUILayout.Width(60)))
                                config.Remove(key);
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox($"No schema registered for type '{type}'", MessageType.Info);
                }
            }

            // Recursive draw for children
            if (nodeObject[CoreKeys.Children] is JArray children)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(CoreKeys.Children, EditorStyles.boldLabel);

                for (var i = 0; i < children.Count; i++)
                {
                    if (children[i] is JObject child && child[CoreKeys.Type] != null)
                        DrawNode(new JProperty($"[{i}]", child), $"{path}[{i}]");
                }
            }

            if (nodeObject[CoreKeys.Child] is JObject singleChild)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Child", EditorStyles.boldLabel);
                DrawNode(new JProperty(CoreKeys.Child, singleChild), $"{path}.{CoreKeys.Child}");
            }
        }
        finally
        {
            EditorGUILayout.EndVertical();
        }
    }

    private static bool _hasRan;
    private void OnEnable()
    {
        if (_hasRan) return;
        _hasRan = true;
        
        BtNodeSchemaRegistrationList.InitializeDefaults();
        // TODO: trigger validation or loading here if needed
    }
}