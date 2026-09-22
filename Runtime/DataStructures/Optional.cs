using System;
using UnityEngine;

[Serializable]
public struct Optional<T>
{
    public bool enabled;
    public T value;

    public Optional(T value) { this.enabled = true; this.value = value; }

    public readonly bool TryGet(out T result)
    {
        result = value;
        return enabled;
    }

    public static Optional<T> None => default;
    public static Optional<T> Some(T v) => new(v);
}
