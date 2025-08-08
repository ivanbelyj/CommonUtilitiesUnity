using System.Collections.Generic;

public static class ListExtensions
{
    /// <summary>
    /// Performs circular rotation of list elements
    /// </summary>
    /// <param name="list">List to rotate</param>
    /// <param name="shift">Number of positions to shift:
    /// positive - rotate right, negative - rotate left</param>
    public static void Rotate<T>(this List<T> list, int shift)
    {
        if (list == null || list.Count < 2)
        {
            return;
        }

        shift %= list.Count;
        if (shift < 0)
        {
            shift += list.Count;
        }

        if (shift == 0)
        {
            return;
        }

        var temp = new T[list.Count];
        list.CopyTo(temp);

        for (int i = 0; i < list.Count; i++)
        {
            int newIndex = (i + shift) % list.Count;
            list[i] = temp[newIndex];
        }
    }
}