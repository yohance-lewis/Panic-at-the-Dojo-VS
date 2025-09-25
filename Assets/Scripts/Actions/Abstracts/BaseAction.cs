using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;
    protected Unit unit;
    protected bool isActive = false;
    protected Action onActionComplete;
    protected int actionPointCost = 1;
    protected string actionName;
    protected List<HexAxial> validHexAxialList;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
        validHexAxialList = new();
    }

    // ------------ ABSTRACT FUNCTIONS -------------------------------------------------------
    public abstract void TakeAction(HexAxial hexAxial, Action onActionComplete);
    public abstract void GenerateValidActionHexAxialList();

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public virtual bool IsValidActionHexAxial(HexAxial hexAxial)
    {
        return GetValidActionHexAxialList().Contains(hexAxial);
    }

    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void BFS(List<HexAxial> validHexAxialList)
    {
        List<HexAxial> visitedHexAxialList = new();
        Queue<HexAxial> hexesToVisit = new();
        Dictionary<HexAxial, int> levelDictionary = new();
        HexAxial unitHexAxial = unit.GetHexAxial();

        visitedHexAxialList.Add(unitHexAxial);
        hexesToVisit.Enqueue(unitHexAxial);
        levelDictionary.Add(unitHexAxial, 0);
        while (hexesToVisit.Count > 0)
        {
            HexAxial currentHex = hexesToVisit.Dequeue();
            int newLevel = levelDictionary[currentHex] + 1;
            foreach (Neighbor neighbor in LevelGridHex.Instance.GetNeighbors(currentHex))
            {
                if (!visitedHexAxialList.Contains(neighbor.neighborHex))
                {
                    visitedHexAxialList.Add(neighbor.neighborHex);
                    CheckValidityAndSearchability(neighbor.neighborHex, unitHexAxial, newLevel, hexesToVisit, validHexAxialList, levelDictionary);
                }
            }
        }
    }

    protected virtual void CheckValidityAndSearchability(HexAxial hexAxial, HexAxial unitHexAxial, int newLevel, Queue<HexAxial> q, List<HexAxial> validHexAxialList, Dictionary<HexAxial, int> levelDict)
    {}
    // ------------ GETTERS -------------------------------------------------------
    public List<HexAxial> GetValidActionHexAxialList()
    {
        return validHexAxialList;
    }
    public int GetActionPointCost()
    {
        return actionPointCost;
    }

    public string GetActionName()
    {
        return actionName;
    }

    public Unit GetUnit()
    {
        return unit;
    }

}
