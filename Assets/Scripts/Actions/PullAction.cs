using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PullAction : ForcedMovement
{
    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        actionName = "pull";
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
                Vector3 rotationDirection = (unit.GetWorldPosition() - targetUnit.transform.position).normalized;

                targetUnit.transform.forward = Vector3.Lerp(targetUnit.transform.forward, rotationDirection, Time.deltaTime * targetRotationSpeed);
                transform.forward = Vector3.Lerp(transform.forward, -rotationDirection, Time.deltaTime * targetRotationSpeed);

                if (Vector3.Distance(targetPosition, targetUnit.transform.position) > stoppingDistance)
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

    // ------------ OVERRIDES -------------------------------------------------------

    protected override List<HexAxial> GetCandidates()
    {
        List<HexAxial> pullCandidates = new();
        HexAxial startingHex = unit.GetHexAxial();
        int startingDistance = HexUtilities.AxialDistance(startingHex, previousHex);
        foreach (Neighbor neighbor in LevelGridHex.Instance.GetNeighbors(previousHex))
        {
            if (startingDistance == 1)
            {
                if (HexUtilities.AxialDistance(neighbor.neighborHex, startingHex) == 1
                && Pathfinding.Instance.GetNode(neighbor.neighborHex).IsWalkable())
                {
                    pullCandidates.Add(neighbor.neighborHex);
                }

            }
            else if (HexUtilities.AxialDistance(neighbor.neighborHex, startingHex) < startingDistance
            && Pathfinding.Instance.GetNode(neighbor.neighborHex).IsWalkable())
            {
                pullCandidates.Add(neighbor.neighborHex);
            }

        }
        return pullCandidates;
    }
}
