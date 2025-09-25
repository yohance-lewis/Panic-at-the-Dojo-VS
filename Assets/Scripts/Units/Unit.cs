using System;
using UnityEngine;

/**
 * ADT that represents an actionable Unit on the board, whether enemy or player
 * Has operations for flipping cards and returning the board state.
 */

public class Unit : MonoBehaviour, ICanBeDamaged
{

    public static event EventHandler OnAnyActionPointsChanged;
    private HexAxial hexAxial;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    private int currentActionPoints;
    private Team team;
    [SerializeField] private int defautActionPoints = 4;

    [SerializeField] private bool isEnemy;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRender;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        baseActionArray = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        hexAxial = LevelGridHex.Instance.GetHexAxial(transform.position);
        LevelGridHex.Instance.AddUnitAtHexAxial(hexAxial, this);
        TurnSystem.Instance.OnPhaseChanged += TurnSystem_OnPhaseChanged;
        healthSystem.OnDeath += HealthSystem_OnDeath;
        GenerateValidHexlist();
        ResetActionPoints();
    }

    private void Update()
    {
        HexAxial newHexAxial = LevelGridHex.Instance.GetHexAxial(transform.position);
        if (newHexAxial != hexAxial)
        {
            LevelGridHex.Instance.UnitMovedHexAxial(this, hexAxial, newHexAxial);
            hexAxial = newHexAxial;
        }
    }

    private void OnDisable()
    {
        team.CheckAndHandleDefeat();
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        return currentActionPoints >= baseAction.GetActionPointCost();
    }

    private void SpendActionPoints(int amount)
    {
        currentActionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetColor(Color color)
    {
        skinnedMeshRender.material.color = color;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        return false;
    }

    private void ResetActionPoints()
    {
        currentActionPoints = defautActionPoints;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public void SetTeam(Team team)
    {
        this.team = team;
        SetColor(team.GetTeamColor());
    }

    public void GenerateValidHexlist()
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            baseAction.GenerateValidActionHexAxialList();
        }
    }

    // ------------ GETTERS -------------------------------------------------------
    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T t)
            {
                return t;
            }
        }
        return null;
    }

    public Team GetTeam()
    {
        return team;
    }

    public HexAxial GetHexAxial()
    {
        return hexAxial;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public int GetCurrentActionPoints()
    {
        return currentActionPoints;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public Color GetColor()
    {
        return skinnedMeshRender.material.color;
    }

    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void TurnSystem_OnPhaseChanged(TurnSystem.Phase currentPhase)
    {
        switch (currentPhase)
        {
            case TurnSystem.Phase.UNITSELECTION:
                if (team.IsCurrentTeam())
                {
                    ResetActionPoints();
                }
                break;
            case TurnSystem.Phase.PLAY:
                GenerateValidHexlist();
                break;
        }
    }

    private void HealthSystem_OnDeath(object sender, EventArgs e)
    {
        LevelGridHex.Instance.RemoveUnitAtHexAxial(hexAxial, this);
        team.KillUnit(this);
        gameObject.SetActive(false);
    }
}
