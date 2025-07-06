using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Renders a $ref dropdown that pulls available keys from a specific params section
/// (e.g., "timing" or "movement") of the BtConfig plugin in the loaded entity config.
/// </summary>
public class RefSelectorFieldRenderer : IJsonFieldRenderer
{
    // Holds the full "params" JObject from the entity config (should be set after loading/applying).
    private JObject _paramsRoot;

    public void SetParamRoot(JObject btConfigRoot)
    {
        _paramsRoot = btConfigRoot;
    }
    
    public bool CanRender(BtNodeSchemaField field)
    {
        // Only fields marked as allowRef should use this renderer
        return field.AllowRef;
    }
    
    public JToken Render(string key, JToken currentValue, BtNodeSchemaField schemaField)
    {
        var options = new List<string>();
        var currentRef = "";

        // Extract current $ref value if any
        if (currentValue != null && currentValue.Type == JTokenType.Object && currentValue[BtJsonFields.Ref] != null)
            currentRef = currentValue[BtJsonFields.Ref].ToString();

        var isBlockMode = schemaField.RefType == RefSelectorType.Block;
        var blockRefPresent = !string.IsNullOrEmpty(currentRef);
        
        // If block mode, show only if current value is a block
        if (isBlockMode)
        {
            // Only top-level params keys (movement, timing, targeting, etc.)
            if (_paramsRoot != null)
                options.AddRange(_paramsRoot.Properties().Select(p => p.Name));
            
            // If current value not present, add for display
            if (!string.IsNullOrEmpty(currentRef) && !options.Contains(currentRef))
                options.Add(currentRef);
            
            if (options.Count == 0)
                options.Add("(no options)");
            
            var currentIndex = !string.IsNullOrEmpty(currentRef) ? options.IndexOf(currentRef) : 0;
            var newIndex = EditorGUILayout.Popup(
                ObjectNames.NicifyVariableName(key) + $" {BtJsonFields.Ref}",
                Mathf.Max(0, currentIndex),
                options.ToArray()
            );
            
            var manualValue = options[newIndex];
            manualValue = EditorGUILayout.TextField($"Manual {BtJsonFields.Ref}", manualValue);

            if (!string.IsNullOrEmpty(manualValue) && manualValue != "(no options)")
            {
                // When using block ref, you should **disable per-field** in the parent editor (BtEditorWindow)
                return new JObject { [BtJsonFields.Ref] = manualValue };
            }
            
            return currentValue;
        }
        
        // --- Field Ref Mode ---
        // Only from the ParamSection (e.g., "movement", "timing")
        var sectionName = schemaField.ParamSection;
        
        if (!string.IsNullOrEmpty(sectionName) && _paramsRoot != null)
        {
            var sectionToken = _paramsRoot[sectionName] as JObject;
            if (sectionToken != null)
                options.AddRange(sectionToken.Properties().Select(p => $"{sectionName}.{p.Name}"));
        }
        
        if (options.Count == 0)
            options.Add("(no options)");
        
        // If current value is missing, add for display
        if (!string.IsNullOrEmpty(currentRef) && !options.Contains(currentRef))
            options.Add(currentRef);
        
        var fieldCurrentIndex = !string.IsNullOrEmpty(currentRef) ? options.IndexOf(currentRef) : 0;
        var fieldNewIndex = EditorGUILayout.Popup(
            ObjectNames.NicifyVariableName(key) + $" {BtJsonFields.Ref}",
            Mathf.Max(0, fieldCurrentIndex),
            options.ToArray()
        );
        
        var fieldManualValue = options[fieldNewIndex];
        fieldManualValue = EditorGUILayout.TextField($"Manual {BtJsonFields.Ref}", fieldManualValue);

        if (!string.IsNullOrEmpty(fieldManualValue) && fieldManualValue != "(no options)")
        {
            // When setting a field-level ref, you should **remove any block-level $ref** in the parent editor (BtEditorWindow)
            return new JObject { [BtJsonFields.Ref] = fieldManualValue };
        }

        return currentValue;
    }
}