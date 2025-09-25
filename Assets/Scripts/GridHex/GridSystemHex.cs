using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;

public abstract class GridSystemHex<TGridObject> where TGridObject : IHasNeighbors
{
    private readonly float sizeOffset = Mathf.Sqrt(3);
    protected Dictionary<HexAxial, TGridObject> gridObjectDict;
    protected int maxQ;
    protected int maxR;
    protected float maxDistance;

    // ------------ CONSTRUCTOR -------------------------------------------------------
    public GridSystemHex() { }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void CreateDebugObjects(Transform debugPrefab)
    {
        foreach (HexAxial testHex in gridObjectDict.Keys)
        {
            float yLayer = 0.03f;
            Vector3 instantiationPosition = GetWorldPosition(testHex);
            instantiationPosition.y = yLayer;
            Transform debugTransform = UnityEngine.Object.Instantiate(debugPrefab, instantiationPosition, Quaternion.identity);
            GridDebugObjectHex gridDebugObjectHex = debugTransform.GetComponent<GridDebugObjectHex>();
            gridDebugObjectHex.SetGridObjectHex(GetGridObjectHex(testHex));
        }
    }

    public void CreateWalls(List<HexAxial> wallList, Transform wallPrefab)
    {
        foreach (HexAxial testHex in wallList)
        {
            Vector3 instantiationPosition = GetWorldPosition(testHex);
            UnityEngine.Object.Instantiate(wallPrefab, instantiationPosition, Quaternion.identity);
        }
    }

    public void CreateRubble(List<HexAxial> wallList, Transform wallPrefab)
    {
        foreach (HexAxial testHex in wallList)
        {
            Vector3 instantiationPosition = GetWorldPosition(testHex);
            UnityEngine.Object.Instantiate(wallPrefab, instantiationPosition, Quaternion.identity);
        }
    }

    protected void BuildRow(HexAxial start, int n, Func<GridSystemHex<TGridObject>, HexAxial, TGridObject> createGridObject)
    {
        for (int i = 0; i < n; i++)
        {
            HexAxial newHex = start + HexUtilities.neighbors[HexUtilities.Direction.EAST] * i;
            gridObjectDict.Add(newHex, createGridObject(this, newHex));
        }
    }

    protected void AddNeighbor(HexAxial hexAxial, TGridObject gridObjectHex)
    {
        foreach (HexAxial neighborVector in HexUtilities.neighbors.Values)
        {
            HexAxial potentialNeighbor = hexAxial + neighborVector;

            if (gridObjectDict.ContainsKey(potentialNeighbor))
            {
                gridObjectHex.AddNeighbor(potentialNeighbor);
            }
        }

    }

    protected void AddAllNeighbors()
    {
        foreach (KeyValuePair<HexAxial, TGridObject> keyValuePair in gridObjectDict)
        {
            AddNeighbor(keyValuePair.Key, keyValuePair.Value);
        }
    }

    // ------------ GETTERS -------------------------------------------------------
    public TGridObject GetGridObjectHex(HexAxial hexAxial)
    {
        return gridObjectDict[hexAxial];
    }

    public Dictionary<HexAxial, TGridObject>.KeyCollection GetAllHexes()
    {
        return gridObjectDict.Keys;
    }

    public List<Neighbor> GetNeighbors(HexAxial hexAxial)
    {
        return gridObjectDict[hexAxial].GetNeighbors();
    }

    public bool IsValidHexAxial(HexAxial hexAxial)
    {
        return gridObjectDict.ContainsKey(hexAxial);
    }

    public float GetMaxDsitance()
    {
        return maxDistance;
    }

    public Vector3 GetWorldPosition(HexAxial hexAxial)
    {
        int s = hexAxial.q - hexAxial.r;
        float x = sizeOffset * s + sizeOffset / 2 * hexAxial.r;
        float z = 3f / 2 * hexAxial.r;
        return new Vector3(x, 0, z) * 2 / sizeOffset;
    }

    public HexAxial GetHexAxial(Vector3 worldPosition)
    {
        float x = worldPosition.x / (2 / sizeOffset);
        float z = worldPosition.z / (2 / sizeOffset);

        float q = sizeOffset / 3 * x - 1f / 3 * z;
        float r = 2f / 3 * z;
        HexFrac hexFrac = new(-q - r, r, q);
        return HexUtilities.AxialRound(hexFrac);
    }
}
