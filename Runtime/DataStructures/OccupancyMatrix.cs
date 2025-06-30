using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bit matrix that stores data about grid occupancy
/// </summary>
[Serializable]
public class OccupancyMatrix
{
    [Serializable]
    public struct OccupiedRect : IEquatable<OccupiedRect>
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public bool Equals(OccupiedRect other)
        {
            return x == other.x && y == other.y && width == other.width && height == other.height;
        }
    }

    private BitArray[] matrixData;
    private int columns;
    private int rows;

    public int Width => columns;
    public int Height => rows;

    public OccupancyMatrix(int rows, int columns)
    {
        matrixData = new BitArray[rows];
        this.columns = columns;
        this.rows = rows;

        for (int i = 0; i < rows; i++)
        {
            matrixData[i] = new BitArray(columns);
        }
    }

    public bool this[int row, int column]
    {
        get => matrixData[row][column];
        private set => matrixData[row][column] = value;
    }

    public void SetRect(OccupiedRect rect, bool value)
    {
        SetRect(rect.width, rect.height, rect.x, rect.y, value);
    }

    /// <summary>
    /// Sets the given value in the occupancy matrix positions corresponding to the size 
    /// and position of the rectangle
    /// </summary>
    public void SetRect(int width, int height, int x, int y, bool value)
    {
        for (int row = y; row < y + height; row++)
        {
            for (int column = x; column < x + width; column++)
            {
                this[row, column] = value;
            }
        }
    }

    /// <summary>
    /// Returns true if the rectangle is within the matrix bounds
    /// </summary>
    public bool IsRectInBounds(int width, int height, int x, int y)
    {
        return x >= 0 && y >= 0
            && x + width <= columns
            && y + height <= rows;
    }

    public bool IsRectInBounds(OccupiedRect rect)
    {
        return IsRectInBounds(rect.width, rect.height, rect.x, rect.y);
    }

    /// <summary>
    /// Checks whether the space in the occupancy matrix is free for the given rectangle.
    /// Returns true if the space is available
    /// </summary>
    public bool HasSpaceForRect(int width, int height, int x, int y)
    {
        // If the checked area is out of bounds, the rectangle
        // definitely won't fit as it's limited by matrix borders
        if (!IsRectInBounds(width, height, x, y))
        {
            return false;
        }

        for (int row = y; row < y + height; row++)
        {
            for (int column = x; column < x + width; column++)
            {
                // If position is occupied, the item can't be placed in this area
                if (this[row, column])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool HasSpaceForRect(OccupiedRect rect)
    {
        return HasSpaceForRect(rect.width, rect.height, rect.x, rect.y);
    }

    /// <summary>
    /// Finds free space for a rectangle of given size.
    /// Returns true if such space was found in the section.
    /// Time complexity: O(n^2)
    /// </summary>
    public bool FindFreePositionForRect(int width, int height, out int x, out int y)
    {
        for (int row = 0; row < matrixData.Length; row++)
        {
            for (int column = 0; column < this.columns; column++)
            {
                if (HasSpaceForRect(width, height, column, row))
                {
                    x = column;
                    y = row;
                    return true;
                }
            }
        }
        x = y = 0;
        return false;
    }

    /// <summary>
    /// Creates an occupancy matrix based on data about its current occupancy.
    /// Time complexity: O(n)
    /// </summary>
    public static OccupancyMatrix Create(int rows, int columns, OccupiedRect[] occupiedRects)
    {
        OccupancyMatrix result = new OccupancyMatrix(rows, columns);
        foreach (var rect in occupiedRects)
        {
            // In practice can be considered O(const) since average size
            // of occupancy rectangle is constant
            result.SetRect(rect, true);
        }
        return result;
    }

    /// <summary>
    /// Returns a copy of the rectangle that extends beyond the occupancy matrix bounds,
    /// clipped to the matrix borders.
    /// </summary>
    public OccupiedRect ClipToBounds(OccupiedRect rect)
    {
        int rowOverflow = (rect.height + rect.y) - rows;
        int columnOverflow = (rect.width + rect.x) - columns;
        return new OccupiedRect()
        {
            x = rect.x,
            y = rect.y,
            width = rowOverflow > 0 ? rect.width - columnOverflow : rect.width,
            height = columnOverflow > 0 ? rect.height - rowOverflow : rect.height
        };
    }
}
