using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Team
{
    public static int MAXTEAMSIZE = 3;
    public static Action OnAnyTeamDefeated;
    private enum UnitState
    {
        ALIVE,
        DEAD
    }
    private Dictionary<Unit, UnitState> units;
    private Color teamColor;

    // ------------ CONSTRUCTOR -------------------------------------------------------
    public Team(Color teamColor)
    {
        units = new();
        this.teamColor = teamColor;
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void AddUnit(Unit unit)
    {
        units.TryAdd(unit, UnitState.ALIVE);
    }

    public void KillUnit(Unit unit)
    {
        if (units.ContainsKey(unit))
        {
            units[unit] = UnitState.DEAD;
        }
    }

    public void CheckAndHandleDefeat()
    {
        if (GetDeadUnits().Count == units.Count)
        {
            OnAnyTeamDefeated.Invoke();
        }
    }

    // ------------ GETTERS -------------------------------------------------------
    public Color GetTeamColor()
    {
        return teamColor;
    }

    public bool IsCurrentTeam()
    {
        return TurnSystem.Instance.GetCurrentTeam() == this;
    }

    public List<Unit> GetUnits()
    {
        return units.Keys.ToList();
    }

    public List<Unit> GetAliveUnits()
    {
        return units.Keys.Where(x => units[x] == UnitState.ALIVE).ToList();
    }

    public List<Unit> GetDeadUnits()
    {
        return units.Keys.Where(x => units[x] == UnitState.DEAD).ToList();
    }

    public bool IsFull()
    {
        return units.Count == MAXTEAMSIZE;
    }
}