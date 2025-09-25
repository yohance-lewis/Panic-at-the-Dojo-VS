using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : OffensiveAction
{
    public event EventHandler<OnShootEventArgs> OnShoot;
    public static event EventHandler<List<ICanBeDamaged>> OnWaitingForTarget;
    public class OnShootEventArgs : EventArgs
    {
        public ICanBeDamaged targetUnit;
        public Unit shootingUnit;

        public Action<int> dealDamage;
        public int damageAmount;
    }
    [SerializeField] private int shootDamage = 2;
    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        actionPointCost = 1;
        actionName = "shoot";
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.Aiming:
                Vector3 aimDirection = (target.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * attackerRotationSpeed);
                break;
            case State.Attacking:
                if (canAttack)
                {
                    Shoot();
                    canAttack = false;
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
    public override void TakeAction(HexAxial hexAxial, Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        List<ICanBeDamaged> potentialTargets = new();

        Unit potentialUnit = LevelGridHex.Instance.GetUnitAtHexAxial(hexAxial);
        if (potentialUnit != null)
        {
            potentialTargets.Add(potentialUnit);
        }
        Obstacle potentialObstacle = LevelGridHex.Instance.GetObstacleAtHexAxial(hexAxial);
        if (potentialObstacle != null)
        {
            potentialTargets.Add(potentialObstacle);
        }

        if (potentialTargets.Count == 1)
        {
            StartShoot(potentialTargets[0]);
        }
        else
        {
            OnWaitingForTarget?.Invoke(this, potentialTargets);
        }
    }

    protected override void CheckValidityAndSearchability(HexAxial hexAxial, HexAxial unitHexAxial, int newLevel, Queue<HexAxial> q, List<HexAxial> validHexAxialList, Dictionary<HexAxial, int> levelDict)
    {
        if (newLevel < maxRange)
        {
            q.Enqueue(hexAxial);
            levelDict.Add(hexAxial, newLevel);
        }

        if (!IsTargetable(unitHexAxial, hexAxial))
        {
            return;
        }
        if (!LevelGridHex.Instance.IsValidHexAxial(hexAxial)
                || HexUtilities.AxialDistance(unitHexAxial, hexAxial) > maxRange
                || HexUtilities.AxialDistance(unitHexAxial, hexAxial) < minRange)
        {
            return;
        }

        if (LevelGridHex.Instance.IsEmpty(hexAxial))
        {
            rangeHexAxialList.Add(hexAxial);
            return;
        }


        if (!LevelGridHex.Instance.HasObstacleOnHexAxial(hexAxial))
        {
            Unit targetUnit = LevelGridHex.Instance.GetUnitAtHexAxial(hexAxial);
            if (targetUnit.GetTeam() == unit.GetTeam())
            {
                return;
            }
        }

        validHexAxialList.Add(hexAxial);
    }
    
    // ------------ NEW FUNCTIONS -------------------------------------------------------
    private void Shoot()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = target,
            shootingUnit = unit,
            dealDamage = target.Damage,
            damageAmount = shootDamage
        });
    }
    public void StartShoot(ICanBeDamaged target)
    {
        this.target = target;
        StartAttack();
    }
}
