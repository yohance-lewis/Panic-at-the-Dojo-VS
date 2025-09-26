using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif
using UnityEngine;

public class GridSystemHexHexagon<TGridObject>: GridSystemHex<TGridObject> where TGridObject: IHasNeighbors
{
    // ------------ CONSTRUCTOR -------------------------------------------------------
    public GridSystemHexHexagon(int q, int r, int s, Func<GridSystemHex<TGridObject>, HexAxial, TGridObject> createGridObject)
    {
        maxQ = q + s - 2;
        maxR = r + s - 2;
        gridObjectDict = new();
        int n = r + s - 1;
        int noRows = q;
        HexAxial prevStart = new(0, 0);
        for (int i = 0; i < n; i++)
        {
            BuildRow(prevStart, noRows, createGridObject);
            if (i < r - 1)
            {
                prevStart += HexUtilities.neighbors[HexUtilities.Direction.NORTHWEST];
                noRows += 1;
            }
            else
            {
                prevStart += HexUtilities.neighbors[HexUtilities.Direction.NORTHEAST];
            }
            if (i >= s - 1)
            {
                noRows -= 1;
            }
        }
        AddAllNeighbors();
        maxDistance = Vector3.Distance(GetWorldPosition(new(0, 0)),GetWorldPosition(new(maxQ, maxR)));
    }
}
