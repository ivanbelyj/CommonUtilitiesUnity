using UnityEngine;
using System;

using Object = UnityEngine.Object;

// Todo: rename to InterfaceReference in a separate commit.
[System.Serializable]
public struct InterfaceField<T> where T : class
{
    [SerializeField] private Object _unityObject;
    
    public T Value
    {
        get
        {
            if (_unityObject == null)
                return null;

            if (_unityObject is T direct)
                return direct;

            if (_unityObject is GameObject go)
            {
                if (!go.TryGetComponent<T>(out var component))
                {
#if UNITY_EDITOR
                    Debug.LogError(
                        $"[{nameof(InterfaceField<T>)}<{typeof(T).Name}>] " +
                        $"GameObject '{go.name}' has no component implementing '{typeof(T).Name}'.",
                        go);
#endif
                }
                return component;
            }

#if UNITY_EDITOR
            Debug.LogError(
                $"[{nameof(InterfaceField<T>)}<{typeof(T).Name}>] " +
                $"Object '{_unityObject.name}' ({_unityObject.GetType().Name}) " +
                $"does not implement '{typeof(T).Name}' and is not a GameObject.",
                _unityObject);
#endif
            return null;
        }
        set
        {
            if (value == null)
            {
                _unityObject = null;
                return;
            }

            if (value is Object unityObject)
            {
                _unityObject = unityObject;
                return;
            }

#if UNITY_EDITOR
            Debug.LogError(
                $"[{nameof(InterfaceField<T>)}<{typeof(T).Name}>] " +
                $"Value of type '{value.GetType().Name}' is not a UnityEngine.Object. " +
                $"Assign a Component, ScriptableObject or GameObject.",
                value as Object);
#endif
            _unityObject = null;
        }
    }

    public static implicit operator T(InterfaceField<T> field) => field.Value;
    public static implicit operator InterfaceField<T>(T value) => new() { _unityObject = value as Object };
}