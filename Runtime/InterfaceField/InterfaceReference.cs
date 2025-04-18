using UnityEngine;
using System;

using Object = UnityEngine.Object;

[System.Serializable]
public struct InterfaceField<T> where T : class
{
    [SerializeField] private Object _unityObject;
    
    public T Value
    {
        get
        {
            if (_unityObject is T direct) return direct;
            if (_unityObject is GameObject go) return go.GetComponent<T>();
            return null;
        }
        set => _unityObject = value as Object;
    }

    public static implicit operator T(InterfaceField<T> field) => field.Value;
    public static implicit operator InterfaceField<T>(T value) => new() { _unityObject = value as Object };
}