using System.Collections.Generic;
using UnityEngine;

public class PathNode : IHasNeighbors
{
    private HexAxial hexAxial;
    private int gCost;
    private int hCost;
    private int fCost;

    private PathNode cameFromPathNode;
    private List<HexAxial> neighbors;
    private bool isWalkable = true;

    // ------------ CONSTRUCTOR -------------------------------------------------------
    public PathNode(HexAxial hexAxial)
    {
        this.hexAxial = hexAxial;
        neighbors = new List<HexAxial>();
    }

    // ------------ TOSTRING -------------------------------------------------------
    public override string ToString()
    {
        return hexAxial.ToString();
    }
    // ------------ INTERFACE IMPLEMENTATIONS -------------------------------------------------------
    public void AddNeighbor(HexAxial hexAxial)
    {
        neighbors.Add(hexAxial);
    }
    public void RemoveNeighbor(HexAxial hexAxial)
    {
        neighbors.Remove(hexAxial);
    }

    public List<Neighbor> GetNeighbors()
    {
        List < Neighbor > neighborList= new();
        int cost = 1;
        if (LevelGridHex.Instance.HasRubbleOnHexAxial(hexAxial)) { cost = 2; }

        foreach (HexAxial neighborNode in neighbors)
        {
            neighborList.Add(new(neighborNode, cost));
        }
        return neighborList;
    }
    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void CalculateFCost()
    {
        fCost = hCost + gCost;
    }

    public void ResetCameFromPathNode()
    {
        cameFromPathNode = null;
    }

    // ------------ GETTERS -------------------------------------------------------
    public int GetGCost()
    {
        return gCost;
    }
    public int GetFCost()
    {
        return fCost;
    }
    public int GetHCost()
    {
        return hCost;
    }

    public PathNode GetCameFromPathNode()
    {
        return cameFromPathNode;
    }

    public HexAxial GetHexAxial()
    {
        return hexAxial;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }

    // ------------ SETTERS -------------------------------------------------------
    public void SetGCost(int gCost)
    {
        this.gCost = gCost;
    }

    public void SetHCost(int hCost)
    {
        this.hCost = hCost;
    }

    public void SetCameFromPathNode(PathNode pathNode)
    {
        cameFromPathNode = pathNode;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
}
