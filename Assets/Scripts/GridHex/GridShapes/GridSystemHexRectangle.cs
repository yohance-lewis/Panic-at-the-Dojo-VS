using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;

public class GridSystemHexRectangle<TGridObject> : GridSystemHex<TGridObject> where TGridObject : IHasNeighbors
{
    public GridSystemHexRectangle(int width, int height, Func<GridSystemHex<TGridObject>, HexAxial, TGridObject> createGridObject)
    {
        maxQ = width + height / 2 - 1;
        maxR = height - 1;
        gridObjectDict = new();
        HexAxial prevStart = new(0, 0);
        for (int i = 0; i < height; i++)
        {
            BuildRow(prevStart, width, createGridObject);
            if (i % 2 == 0)
            {
                prevStart += HexUtilities.neighbors[HexUtilities.Direction.NORTHEAST];
            }
            else
            {
                prevStart += HexUtilities.neighbors[HexUtilities.Direction.NORTHWEST];
            }
        }
        AddAllNeighbors();
        maxDistance = Vector3.Distance(GetWorldPosition(new(0, 0)),GetWorldPosition(new(maxQ, maxR)));
    }
}
