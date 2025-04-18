#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(InterfaceField<>))]
public class InterfaceFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var objectProp = property.FindPropertyRelative("_unityObject");
        Type interfaceType = fieldInfo.FieldType.GetGenericArguments()[0];
        
        EditorGUI.BeginChangeCheck();
        var newValue = EditorGUI.ObjectField(position, label, objectProp.objectReferenceValue, typeof(Object), true);
        
        if (EditorGUI.EndChangeCheck())
        {
            objectProp.objectReferenceValue = IsValidObject(newValue, interfaceType) ? newValue : null;
        }

        if (objectProp.objectReferenceValue != null && !IsValidObject(objectProp.objectReferenceValue, interfaceType))
        {
            position.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.HelpBox(position, $"Object must implement {interfaceType.Name}", MessageType.Error);
        }
    }

    private bool IsValidObject(Object obj, Type interfaceType)
    {
        if (obj == null) return false;
        if (interfaceType.IsAssignableFrom(obj.GetType())) return true;
        return obj is GameObject go && go.GetComponent(interfaceType) != null;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var objectProp = property.FindPropertyRelative("_unityObject");
        Type interfaceType = fieldInfo.FieldType.GetGenericArguments()[0];
        
        return (objectProp.objectReferenceValue == null || IsValidObject(objectProp.objectReferenceValue, interfaceType)) 
            ? EditorGUIUtility.singleLineHeight 
            : EditorGUIUtility.singleLineHeight * 2 + 2;
    }
}
#endif