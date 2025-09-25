using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGridHex : MonoBehaviour
{
    public static LevelGridHex Instance { get; private set; }
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private List<HexAxial> wallList;
    [SerializeField] private Transform wallPrefab;
    [SerializeField] private List<HexAxial> rubbleList;
    [SerializeField] private Transform rubblePrefab;
    [SerializeField] private int q;
    [SerializeField] private int r;
    [SerializeField] private int s;
    [SerializeField] private bool isHex;
    [SerializeField] private bool createDebugObjects;
    private GridSystemHex<GridObjectHex> gridSystemHex;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (isHex)
        {
            gridSystemHex = new GridSystemHexHexagon<GridObjectHex>(q, r, s, (g, hexAxial) => new GridObjectHex(g, hexAxial));
        }
        else
        {
            gridSystemHex = new GridSystemHexRectangle<GridObjectHex>(q, r, (g, hexAxial) => new GridObjectHex(g, hexAxial));
        }

        if (createDebugObjects)
        {
            gridSystemHex.CreateDebugObjects(gridDebugObjectPrefab);
        }

    }

    private void Start()
    {
        Pathfinding.Instance.Setup(q, r, s, isHex);
        gridSystemHex.CreateWalls(wallList, wallPrefab);
        gridSystemHex.CreateRubble(rubbleList, rubblePrefab);
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void AddUnitAtHexAxial(HexAxial hexAxial, Unit unit)
    {
        GridObjectHex gridObjectHex = gridSystemHex.GetGridObjectHex(hexAxial);
        gridObjectHex.AddUnit(unit);
        Pathfinding.Instance.GetNode(hexAxial).SetIsWalkable(false);
    }

    public List<Unit> GetUnitListAtHexAxial(HexAxial hexAxial)
    {
        GridObjectHex gridObjectHex = gridSystemHex.GetGridObjectHex(hexAxial);
        return gridObjectHex.GetUnitList();
    }

    public Unit GetUnitAtHexAxial(HexAxial hexAxial)
    {
        GridObjectHex gridObjectHex = gridSystemHex.GetGridObjectHex(hexAxial);
        return gridObjectHex.GetUnit();
    }

    public void RemoveUnitAtHexAxial(HexAxial hexAxial, Unit unit)
    {
        GridObjectHex gridObjectHex = gridSystemHex.GetGridObjectHex(hexAxial);
        gridObjectHex.RemoveUnit(unit);
        Pathfinding.Instance.GetNode(hexAxial).SetIsWalkable(true);
    }

    public Obstacle GetObstacleAtHexAxial(HexAxial hexAxial)
    {
        GridObjectHex gridObjectHex = gridSystemHex.GetGridObjectHex(hexAxial);
        return gridObjectHex.GetObstacle();
    }

    public void RemoveWallAtHexAxial(HexAxial hexAxial)
    {
        GridObjectHex gridObjectHex = gridSystemHex.GetGridObjectHex(hexAxial);
        Pathfinding.Instance.GetNode(hexAxial).SetIsWalkable(true);
    }

    public void AddWallAtHexAxial(HexAxial hexAxial, Wall wall)
    {
        GridObjectHex gridObjectHex = gridSystemHex.GetGridObjectHex(hexAxial);
        gridObjectHex.AddObstacle(wall);
        Pathfinding.Instance.GetNode(hexAxial).SetIsWalkable(false);
    }

    public void RemoveRubbleAtHexAxial(HexAxial hexAxial)
    {
        GridObjectHex gridObjectHex = gridSystemHex.GetGridObjectHex(hexAxial);
        gridObjectHex.RemoveObstacle();
        GridSystemVisual.Instance.GetAllObstacles().Remove(hexAxial);
    }

    public void AddRubbleAtHexAxial(HexAxial hexAxial, Rubble rubble)
    {
        GridObjectHex gridObjectHex = gridSystemHex.GetGridObjectHex(hexAxial);
        gridObjectHex.AddObstacle(rubble);
    }

    public void UnitMovedHexAxial(Unit unit, HexAxial fromHexAxial, HexAxial toHexAxial)
    {
        RemoveUnitAtHexAxial(fromHexAxial, unit);
        AddUnitAtHexAxial(toHexAxial, unit);
    }

    // ------------ PASSTHROUGH FUNCTIONS -------------------------------------------------------
    public HexAxial GetHexAxial(Vector3 worldPosition) => gridSystemHex.GetHexAxial(worldPosition);
    public Vector3 GetWorldPosition(HexAxial hexAxial) => gridSystemHex.GetWorldPosition(hexAxial);
    public bool IsValidHexAxial(HexAxial hexAxial) => gridSystemHex.IsValidHexAxial(hexAxial);
    public List<Neighbor> GetNeighbors(HexAxial hexAxial) => gridSystemHex.GetNeighbors(hexAxial);
    public float GetMaxDsitance() => gridSystemHex.GetMaxDsitance();
    public bool HasAnyUnitOnHexAxial(HexAxial hexAxial) => gridSystemHex.GetGridObjectHex(hexAxial).HasAnyUnit();
    public bool HasWallOnHexAxial(HexAxial hexAxial) => gridSystemHex.GetGridObjectHex(hexAxial).HasWall();
    public bool HasRubbleOnHexAxial(HexAxial hexAxial) => gridSystemHex.GetGridObjectHex(hexAxial).HasRubble();
    public bool HasObstacleOnHexAxial(HexAxial hexAxial) => gridSystemHex.GetGridObjectHex(hexAxial).HasObstacle();
    public bool IsEmpty(HexAxial hexAxial) => gridSystemHex.GetGridObjectHex(hexAxial).IsEmpty();
    public Dictionary<HexAxial, GridObjectHex>.KeyCollection GetAllHexes() => gridSystemHex.GetAllHexes();
}
