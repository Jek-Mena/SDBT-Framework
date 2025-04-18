/*
// Currently disabled

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[CustomPropertyDrawer(typeof(NamedFloatField<>), true)]
public class NamedFloatFieldGenericDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var keyProp = property.FindPropertyRelative("Key");
        var valueProp = property.FindPropertyRelative("Value");

        // Get enum type
        var enumType = GetEnumType(keyProp);
        if (enumType == null)
        {
            EditorGUI.LabelField(position, "Invalid enum type");
            EditorGUI.EndProperty();
            return;
        }

        // Detect siblings and collect used keys
        var parentArray = GetParentArray(property);
        var currentKeyIndex = keyProp.enumValueIndex;

        HashSet<int> usedIndexes = new();
        for (int i = 0; i < parentArray.arraySize; i++)
        {
            var element = parentArray.GetArrayElementAtIndex(i);
            if (element.propertyPath == property.propertyPath) continue;

            var otherKey = element.FindPropertyRelative("Key");
            usedIndexes.Add(otherKey.enumValueIndex);
        }

        // Get enum options
        string[] enumNames = Enum.GetNames(enumType);
        int[] enumValues = (int[])Enum.GetValues(enumType);

        // Filter out used keys, except the current one
        List<int> available = enumValues.Where(i => !usedIndexes.Contains(i) || i == currentKeyIndex).ToList();
        string[] display = available.Select(i => enumNames[i]).ToArray();

        // Draw fields
        Rect keyRect = new(position.x, position.y, position.width * 0.5f, position.height);
        Rect valueRect = new(position.x + position.width * 0.5f + 4, position.y, position.width * 0.5f - 4, position.height);

        int selected = available.IndexOf(currentKeyIndex);
        selected = selected < 0 ? 0 : selected;

        int newValue = available[selected];
        newValue = available[EditorGUI.Popup(keyRect, selected, display)];
        keyProp.enumValueIndex = newValue;

        valueProp.floatValue = EditorGUI.FloatField(valueRect, valueProp.floatValue);

        EditorGUI.EndProperty();
    }

    private SerializedProperty GetParentArray(SerializedProperty property)
    {
        string path = property.propertyPath;
        string arrayPath = path[..path.LastIndexOf(".Array.data[", StringComparison.Ordinal)];
        return property.serializedObject.FindProperty(arrayPath);
    }

    private type GetEnumType(SerializedProperty enumProp)
    {
        type parentType = enumProp.serializedObject.targetObject.GetType();
        var fieldInfo = parentType
            .GetField(enumProp.propertyPath.Replace(".Key", ""), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

        if (fieldInfo == null) return null;

        var genericType = fieldInfo.FieldType;
        if (genericType.IsArray)
            genericType = genericType.GetElementType();

        if (genericType.IsGenericType && genericType.GetGenericTypeDefinition() == typeof(NamedFloatField<>))
            return genericType.GetGenericArguments()[0];

        return null;
    }
}
*/