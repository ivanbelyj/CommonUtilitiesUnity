using UnityEngine;

public static class UnityObjectExtensions
{
    // Primary purpose - to check Unity objects which are passed via interface type reference.
    public static bool IsNull<T>(this T @object) where T : class
        => @object == null || @object.Equals(null);

    public static bool IsNotNull<T>(this T @object) where T : class
        => !@object.IsNull();
}
