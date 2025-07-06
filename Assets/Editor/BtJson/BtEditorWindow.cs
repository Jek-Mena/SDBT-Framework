using System.IO;
using AI.BehaviorTree.Keys;
using Editor.BtJson;
using Editor.BtJson.Panel;
using Editor.BtJson.Utilities;
using Keys;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public class BtEditorWindow : EditorWindow
{
    private JObject _treeJson;
    private JObject _selectedNode;
    
    private bool _showRawBtJson = true; // Optionally let user toggle this in the toolbar
    
    private BtFieldRenderDispatcher _fieldRenderDispatcher;
    private RefSelectorFieldRenderer _refSelectorFieldRenderer = new();
    
    // Keep a reference to the config params used for $ref
    private JObject _currentEntityConfigParams;
    
    // Right Main Panel
    private GraphViewPanel _graphViewPanel;
    private NodeInspectorPanel _nodeInspectorPanel;
    
    // Left Main Panel
    private RawEntityJsonPanel _rawEntityJsonPanel;
    private RawBtJsonPanel _rawBtJsonPanel;
    
    private const string BtJsonDirectory = "Assets/Resources/Data";
    private const string BtJsonDirectoryBts = "Assets/Resources/Data/BTs";
    private const string DefaultEntityConfigPath = "Assets/Resources/Data/Units/Enemies/Standard/enemy_standard_laghound.json";
    private const string DefaultBtPath = "Assets/Resources/Data/BTs/move_and_wait.json";
    
    [MenuItem("Tools/Behavior Tree/Open Editor")]
    public static void OpenWindow()
    {
        var fileName = Path.GetFileNameWithoutExtension(DefaultBtPath); 
        GetWindow<BtEditorWindow>($"Behavior Tree Editor - {fileName}");
    }
    
    private void OnEnable()
    {
        // Always register schemas before anything else
        BtNodeSchemaRegistrationList.InitializeDefaults();
        
        // Load the default entity config for $ref dropdowns
        LoadEntityConfig(DefaultEntityConfigPath);
        
        // Load a default BT file (if you want an empty new tree, just comment this and use CreateNewTreeJson)
        if (File.Exists(DefaultBtPath))
            _treeJson = JObject.Parse(File.ReadAllText(DefaultBtPath));
        else
            _treeJson = CreateNewTreeJson();
        
        // Create a new node and configure graph panel
        _graphViewPanel = new GraphViewPanel(_treeJson);
        _graphViewPanel.OnNodeSelected = node => _selectedNode = node;
        
        // Create a new node inspector panel
        _refSelectorFieldRenderer = new RefSelectorFieldRenderer();
        _nodeInspectorPanel = new NodeInspectorPanel(_refSelectorFieldRenderer);

        // Create a new raw entity JSON panel
        _rawEntityJsonPanel = new RawEntityJsonPanel(_refSelectorFieldRenderer);
        _rawBtJsonPanel = new RawBtJsonPanel();
    }
    
    public void OnGUI()
    {
        DrawToolbar();
        
        EditorGUILayout.BeginHorizontal();
        
        // LEFT: Main Editor Panels
        using (new VerticalScopeSafe("MainEditorPanel", null, GUILayout.Width(position.width * 0.5f)))
        {
            _graphViewPanel.Render();
            _nodeInspectorPanel.Render(_selectedNode);
        }

        // RIGHT: Raw JSON Panels (Entity + BT, stacked vertically)
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.5f));
        _rawEntityJsonPanel.Render(_currentEntityConfigParams, updatedParams =>
        {
            _currentEntityConfigParams = updatedParams;
        });
        _rawBtJsonPanel.Render(_treeJson);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        if (GUILayout.Button("New", EditorStyles.toolbarButton))
            _treeJson = CreateNewTreeJson();

        if (GUILayout.Button("Save As", EditorStyles.toolbarButton))
            SaveTreeAs();

        if (GUILayout.Button("Load BT", EditorStyles.toolbarButton))
            LoadTreeFromDisk();
        
        if (GUILayout.Button("Load Entity Config", EditorStyles.toolbarButton))
            LoadEntityConfig(EditorUtility.OpenFilePanel("Load Entity Config", BtJsonDirectory, "json"));
        
        if (GUILayout.Button(_showRawBtJson ? "Hide Raw BT JSON" : "Show Raw BT JSON", EditorStyles.toolbarButton))
            _showRawBtJson = !_showRawBtJson;
        
        EditorGUILayout.EndHorizontal();
    }

    private void LoadTreeFromDisk()
    {
        var path = EditorUtility.OpenFilePanel("Load Behavior Tree JSON", BtJsonDirectory, "json");

        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

        _treeJson = JObject.Parse(File.ReadAllText(path));
        _graphViewPanel.SetTree(_treeJson);
    }
    
    private void SaveTreeAs()
    {
        var path = EditorUtility.SaveFilePanel("Save Behavior Tree JSON", BtJsonDirectory, "NewTree.json", "json");
        if(!string.IsNullOrEmpty(path))
            File.WriteAllText(path, _treeJson.ToString());
    }
    
    [System.Obsolete("Outdated but might be useful for future.")]
    private void LoadEntityConfig(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return;
        
        var configText = File.ReadAllText(path);
        var configRoot = JObject.Parse(configText);
        
        //_currentEntityConfigParams = configRoot[EntityJsonFields.Components]?[0]?[EntityJsonFields.Params] as JObject;
        //_refSelectorFieldRenderer.SetParamRoot(_currentEntityConfigParams);
    }
    
    private JObject CreateNewTreeJson()
    {
        return JObject.FromObject(new
        {
            root = new
            {
                type = BtNodeTypes.Composite.Selector,
                children = new object[] { }
            }
        });
    }

    public JObject CreateNewNode(string nodeType)
    {
        // Get schema
        if (!BtNodeSchemaRegistry.TryGet(nodeType, out var schemaObj))
        {
            Debug.LogError($"No schema found for node type '{nodeType}'");
            return JObject.FromObject(new { type = nodeType });
        }
        
        var config = new JObject();
        foreach (var field in schemaObj.GetFields())
        {
            if (field.DefaultValue != null)
                config[field.Key] = field.DefaultValue.DeepClone();
        }

        var newNode = new JObject
        {
            [BtJsonFields.Type] = nodeType,
            [BtJsonFields.ConfigField] = config
        };
        
        // Add empty children array if node is composite or decorator
        if (schemaObj.SupportsChildren)
            newNode[BtJsonFields.Children] = new JArray();

        return newNode;
    }
}