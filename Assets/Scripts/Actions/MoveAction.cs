using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    private List<Vector3> positionList;
    private int currentPositionIndex;
    [SerializeField] private int maxMoveDistance = 4;
    private readonly float stoppingDistance = 0.03f;
    private readonly float rotateSpeed = 10f;
    private float moveSpeed = 4f;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        actionName = "move";
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        if (Vector3.Distance(targetPosition, transform.position) > stoppingDistance)
        {
            transform.position += moveSpeed * Time.deltaTime * moveDirection;
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
    }

    // ------------ OVERRIDES -------------------------------------------------------
    public override void TakeAction(HexAxial hexAxial, Action onActionComplete)
    {
        List<HexAxial> pathHexAxialList = Pathfinding.Instance.FindPath(unit.GetHexAxial(), hexAxial);
        currentPositionIndex = 0;
        positionList = new();

        foreach(HexAxial pathHexAxial in pathHexAxialList)
        {
            positionList.Add(LevelGridHex.Instance.GetWorldPosition(pathHexAxial));
        }
        
        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public override void GenerateValidActionHexAxialList()
    {
        validHexAxialList = new();
        PriorityQueue<HexAxial, int> hexesToVisit = new();
        HexAxial startHex = unit.GetHexAxial();
        hexesToVisit.Enqueue(startHex,0);
        Dictionary<HexAxial, int> costSoFar = new();
        costSoFar[startHex] = 0;

        while (hexesToVisit.Count > 0)
        {
            HexAxial currentHex = hexesToVisit.Dequeue();

            if (LevelGridHex.Instance.IsValidHexAxial(currentHex)
                && startHex != currentHex
                && Pathfinding.Instance.GetNode(currentHex).IsWalkable()
                && costSoFar[currentHex] <= maxMoveDistance
                )
            {
                validHexAxialList.Add(currentHex);
            }

            if (costSoFar[currentHex] >= maxMoveDistance)
            {
                continue;
            }

            foreach (Neighbor neighbor in LevelGridHex.Instance.GetNeighbors(currentHex))
            {
                int newCost = costSoFar[currentHex] + neighbor.cost;
                HexAxial nextHex = neighbor.neighborHex;
                if ((!costSoFar.ContainsKey(nextHex) || newCost < costSoFar[nextHex])
                    && Pathfinding.Instance.GetNode(nextHex).IsWalkable())
                {
                    costSoFar[nextHex] = newCost;
                    int priority = newCost;
                    hexesToVisit.Enqueue(nextHex, priority);
                }
            }
        }
    }
    
    protected override void CheckValidityAndSearchability(HexAxial hexAxial, HexAxial unitHexAxial, int newLevel,Queue<HexAxial> q, List<HexAxial> validHexAxialList, Dictionary<HexAxial, int> levelDict)
    {
        if (!LevelGridHex.Instance.IsValidHexAxial(hexAxial)
            || unitHexAxial == hexAxial
            || !Pathfinding.Instance.GetNode(hexAxial).IsWalkable()
            )
        {
            return;
        }

        validHexAxialList.Add(hexAxial);

        if (newLevel < maxMoveDistance)
        {
            q.Enqueue(hexAxial);
            levelDict.Add(hexAxial, newLevel);
        }
    }
}
