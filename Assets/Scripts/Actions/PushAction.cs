using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PushAction : ForcedMovement
{

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        actionName = "push";
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;
        Unit targetUnit = target as Unit;
        switch (state)
        {
            case State.Aiming:
                Vector3 aimDirection = (target.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * attackerRotationSpeed);
                targetUnit.transform.forward = Vector3.Lerp(targetUnit.transform.forward, -aimDirection, Time.deltaTime * attackerRotationSpeed);
                break;
            case State.Attacking:
                Vector3 targetPosition = LevelGridHex.Instance.GetWorldPosition(path[currentPositionIndex]);
                Vector3 moveDirection = (targetPosition - targetUnit.transform.position).normalized;

                targetUnit.transform.forward = Vector3.Lerp(targetUnit.transform.forward, -moveDirection, Time.deltaTime * targetRotationSpeed);
            
                if (Vector3.Distance(targetPosition, targetUnit.transform.position) > stoppingDistance) // handles movement
                {
                    targetUnit.transform.position += moveSpeed * Time.deltaTime * moveDirection;
                }
                else
                {
                    currentPositionIndex++;
                    if (currentPositionIndex >= path.Count)
                    {
                        NextState();
                    }
                }
                break;
            case State.Cooloff:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }

    }

    // ------------ OVERRIDE -------------------------------------------------------
    public override void TakeAction(HexAxial hexAxial, Action onActionComplete)
    {
        currentPositionIndex = 0;
        this.onActionComplete = onActionComplete;
        target = LevelGridHex.Instance.GetUnitAtHexAxial(hexAxial);
        previousHex = hexAxial;
        path = new();
        AdvanceSelection();
    }

    protected override List<HexAxial> GetCandidates()
    {
        List<HexAxial> candidates = new();
        HexAxial startingHex = unit.GetHexAxial();
        int startingDistance = HexUtilities.AxialDistance(startingHex, previousHex);
        foreach (Neighbor neighbor in LevelGridHex.Instance.GetNeighbors(previousHex))
        {
            if (HexUtilities.AxialDistance(neighbor.neighborHex, startingHex) > startingDistance
            && Pathfinding.Instance.GetNode(neighbor.neighborHex).IsWalkable())
            {
                candidates.Add(neighbor.neighborHex);
            }
        }
        return candidates;
    }
}
