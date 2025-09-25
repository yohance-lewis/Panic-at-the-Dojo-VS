using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class OffensiveAction : BaseAction
{
    protected enum State
    {
        Aiming,
        Attacking,
        Cooloff,
    }
    protected State state;
    protected readonly float attackerRotationSpeed = 10f;
    protected float stateTimer;
    protected bool canAttack;
    protected float aimingTime = 0.75f;
    protected float attackingTime = 0.1f;
    protected float cooloffTime = 0.5f;
    protected ICanBeDamaged target;
    [SerializeField] protected int maxRange = 6;
    [SerializeField] protected int minRange = 2;
    protected List<HexAxial> rangeHexAxialList;

    // ------------ OVERRIDES -------------------------------------------------------
    public override void GenerateValidActionHexAxialList()
    {
        rangeHexAxialList = new();
        validHexAxialList = new();
        BFS(validHexAxialList);
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    protected virtual void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Attacking;
                stateTimer = attackingTime;
                break;
            case State.Attacking:
                state = State.Cooloff;
                stateTimer = cooloffTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    protected bool IsTargetable(HexAxial unitHexAxial, HexAxial hexAxial)
    {
        float epsilon = 0.01f;
        Vector3 startPoint = LevelGridHex.Instance.GetWorldPosition(unitHexAxial);
        Vector3 endPoint = LevelGridHex.Instance.GetWorldPosition(hexAxial);
        Vector3 dir = (endPoint - startPoint).normalized;
        Vector3 perp = new(dir.z, 0, -dir.x);
        perp *= epsilon;

        int distance = HexUtilities.AxialDistance(unitHexAxial, hexAxial);
        for (int i = 1; i < distance; i++)
        {
            Vector3 lerp1 = Vector3.Lerp(startPoint, endPoint + perp, (float)i / distance);
            Vector3 lerp2 = Vector3.Lerp(startPoint, endPoint - perp, (float)i / distance);

            HexAxial hexLerp1 = LevelGridHex.Instance.GetHexAxial(lerp1);
            HexAxial hexLerp2 = LevelGridHex.Instance.GetHexAxial(lerp2);

            if (LevelGridHex.Instance.HasWallOnHexAxial(hexLerp1) && LevelGridHex.Instance.HasWallOnHexAxial(hexLerp2))
            {
                return false;
            }
        }

        return true;
    }

    protected void StartAttack()
    {
        state = State.Aiming;
        stateTimer = aimingTime;
        canAttack = true;

        ActionStart(onActionComplete);
    }

    // ------------ GETTERS -------------------------------------------------------
    public ICanBeDamaged GetTarget()
    {
        return target;
    }

    public List<HexAxial> GetRangeHexAxialList()
    {
        return rangeHexAxialList;
    }
}
