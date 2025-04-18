using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(RequiredAttribute))]
public class RequiredPropertyDrawer : PropertyDrawer
{
    private const float HelpBoxHeight = 38f;
    private const float Spacing = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect fieldRect = position;
        bool showError = false;

        
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            showError = property.objectReferenceValue == null;
        }
        
        else if (IsInterfaceReference(property))
        {
            var objectProp = property.FindPropertyRelative("_unityObject");
            showError = objectProp == null || objectProp.objectReferenceValue == null;
        }
        
        else
        {
            showError = IsValueNull(property);
        }

        if (showError)
        {
            fieldRect.height = EditorGUIUtility.singleLineHeight;
        }

        EditorGUI.PropertyField(fieldRect, property, label, true);

        if (showError)
        {
            Rect helpBoxRect = position;
            helpBoxRect.y += EditorGUI.GetPropertyHeight(property, label, false) + Spacing;
            helpBoxRect.height = HelpBoxHeight;
            
            EditorGUI.HelpBox(helpBoxRect, $"{property.displayName} is required!", MessageType.Error);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUI.GetPropertyHeight(property, label, true);
        
        bool showError = false;
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            showError = property.objectReferenceValue == null;
        }
        else if (IsInterfaceReference(property))
        {
            var objectProp = property.FindPropertyRelative("_unityObject");
            showError = objectProp == null || objectProp.objectReferenceValue == null;
        }
        else
        {
            showError = IsValueNull(property);
        }

        if (showError)
        {
            height += HelpBoxHeight + Spacing;
        }
        
        return height;
    }

    private bool IsInterfaceReference(SerializedProperty property)
    {
        var type = fieldInfo.FieldType;
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(InterfaceField<>);
    }

    private bool IsValueNull(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.String:
                return string.IsNullOrEmpty(property.stringValue);
            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue == null;
            case SerializedPropertyType.Integer:
                return property.intValue == 0;
            case SerializedPropertyType.Float:
                return Math.Abs(property.floatValue) < float.Epsilon;
            case SerializedPropertyType.Boolean:
                return false;
            case SerializedPropertyType.Enum:
                return property.enumValueIndex == 0;
            default:
                return false;
        }
    }
}