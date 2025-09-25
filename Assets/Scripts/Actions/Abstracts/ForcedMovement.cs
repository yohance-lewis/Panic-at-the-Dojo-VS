using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ForcedMovement : OffensiveAction, IClickableAction
{
    protected List<HexAxial> path;
    protected List<HexAxial> candidates;
    protected HexAxial previousHex;
    [SerializeField] protected int range = 4;
    protected int currentPositionIndex;
    protected readonly float stoppingDistance = 0.03f;
    protected readonly float targetRotationSpeed = 20f;
    protected float moveSpeed = 6f;
    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        attackingTime = int.MaxValue;
    }

    // ------------ ABSTRACT FUNCTIONS -------------------------------------------------------
    protected abstract List<HexAxial> GetCandidates();

    // ------------ OVERRIDES -------------------------------------------------------
    public override void TakeAction(HexAxial hexAxial, Action onActionComplete)
    {
        currentPositionIndex = 0;
        path = new();
        this.onActionComplete = onActionComplete;
        target = LevelGridHex.Instance.GetUnitAtHexAxial(hexAxial);
        previousHex = hexAxial;
        AdvanceSelection();
    }
    protected override void CheckValidityAndSearchability(HexAxial hexAxial, HexAxial unitHexAxial, int newLevel, Queue<HexAxial> q, List<HexAxial> validHexAxialList, Dictionary<HexAxial, int> levelDict)
    {
        if (newLevel < maxRange)
        {
            q.Enqueue(hexAxial);
            levelDict.Add(hexAxial, newLevel);
        }

        if (!IsTargetable(unitHexAxial, hexAxial)) { return;}

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

        if (LevelGridHex.Instance.HasObstacleOnHexAxial(hexAxial)) { return;}

        Unit targetUnit = LevelGridHex.Instance.GetUnitAtHexAxial(hexAxial);
        if (targetUnit.GetTeam() == unit.GetTeam()) { return;}

        previousHex = hexAxial;
        if (GetCandidates().Count == 0) { return;}

        validHexAxialList.Add(hexAxial);
    }
    
    // ------------ INTERFACE IMPLEMENTATION -------------------------------------------------------
    public virtual void OnClick()
    {
        if (isActive) { return; }
        HexAxial mouseHexAxial = MouseWorld.GetHexAxial();
        if (candidates.Contains(mouseHexAxial))
        {
            previousHex = mouseHexAxial;
            AdvanceSelection();
            CheckCompleteness();
        }
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    protected void CheckCompleteness()
    {
        if (path.Count == range + 1 || candidates.Count == 0 || LevelGridHex.Instance.HasRubbleOnHexAxial(GetLastInPath()))
        {
            GridSystemVisual.Instance.HideAllHexAxials();
            StartAttack();
        }
    }
    protected void AdvanceSelection()
    {
        path.Add(previousHex);
        candidates = GetCandidates();
        GridSystemVisual.Instance.HideAllHexAxials();
        GridSystemVisual.Instance.ShowHexAxialList(path, GridSystemVisual.GridVisualType.Green);
        GridSystemVisual.Instance.ShowHexAxialList(candidates, GridSystemVisual.GridVisualType.GreenSoft);
    }

    // ------------ GETTERS -------------------------------------------------------
    public HexAxial GetLastInPath()
    {
        return path[^1];
    }
}