using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Mathematics;

public class GridObjectHex : IHasNeighbors
{
    private readonly GridSystemHex<GridObjectHex> gridSystemHex;
    private HexAxial hexAxial;
    private List<Unit> unitList;
    private List<HexAxial> neighbors;

    private Obstacle obstacle;

    // ------------ CONSTRUCTOR -------------------------------------------------------
    public GridObjectHex(GridSystemHex<GridObjectHex> gridSystemHex, HexAxial hexAxial)
    {
        this.gridSystemHex = gridSystemHex;
        this.hexAxial = hexAxial;
        unitList = new List<Unit>();
        neighbors = new List<HexAxial>();
    }

    // ------------ TOSTRING -------------------------------------------------------
    public override string ToString()
    {
        string unitString = "";
        foreach (Unit unit in unitList)
        {
            unitString += unit + "\n";
        }
        return hexAxial.ToString() + "\n" + unitString;
    }

    // ------------ INTERFACE IMPLEMENTATION -------------------------------------------------------
    public void AddNeighbor(HexAxial hexAxial)
    {
        neighbors.Add(hexAxial);
    }
    public void RemoveNeighbor(HexAxial hexAxial)
    {
        neighbors.Remove(hexAxial);
    }
    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
    }
    public bool HasAnyUnit()
    {
        return unitList.Count > 0;
    }

    public void AddObstacle(Obstacle obstacle)
    {
        this.obstacle = obstacle;
    }

    public void RemoveObstacle()
    {
        obstacle = null;
    }

    // ------------ GETTERS -------------------------------------------------------
    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public Unit GetUnit()
    {
        if (HasAnyUnit())
        {
            return unitList[0];
        }
        return null;
    }

    public List<Neighbor> GetNeighbors()
    {
        List<Neighbor> neighborList = new();
        int cost = 1;
        if (HasRubble()) { cost = 2; }

        foreach (HexAxial neighborNode in neighbors)
        {
            neighborList.Add(new(neighborNode, cost));
        }
        return neighborList;
    }

    public Obstacle GetObstacle()
    {
        return obstacle;
    }

    public bool HasWall()
    {
        return obstacle is Wall;
    }
    public bool HasRubble()
    {
        return obstacle is Rubble;
    }

    public bool HasObstacle()
    {
        return obstacle != null;
    }
    public bool IsEmpty()
    {
        return !HasAnyUnit() && !HasObstacle();
    }
}
