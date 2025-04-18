using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class RequiredFieldValidator
{
    static RequiredFieldValidator()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            ValidateRequiredFields();
        }
    }

    private static void ValidateRequiredFields()
    {
        var components = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (var component in components)
        {
            if (component == null) continue;

            var fields = component.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.GetCustomAttribute<RequiredAttribute>() != null);

            foreach (var field in fields)
            {
                try
                {
                    var value = field.GetValue(component);
                    if (IsNull(value))
                    {
                        LogError(component, field);
                        continue;
                    }

                    if (IsInterfaceReference(field.FieldType))
                    {
                        var interfaceValue = GetInterfaceReferenceValue(value);
                        if (interfaceValue == null)
                        {
                            LogError(component, field, "(InterfaceReference)");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error validating field '{field.Name}' in '{component.name}': {e.Message}");
                }
            }
        }
    }

    private static bool IsNull(object value)
    {
        return value == null || (value is Object obj && obj == null);
    }

    private static bool IsInterfaceReference(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(InterfaceField<>);
    }

    private static object GetInterfaceReferenceValue(object interfaceReference)
    {
        var valueProperty = interfaceReference.GetType().GetProperty("Value");
        return valueProperty?.GetValue(interfaceReference);
    }

    private static void LogError(MonoBehaviour component, FieldInfo field, string suffix = "")
    {
        Debug.LogError(
            $"Required field '{field.Name}{suffix}' is not set in '{component.name}' ({component.GetType().Name})", 
            component);
    }
}