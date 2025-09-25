using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurnSystem : MonoBehaviour
{
    public enum Phase
    {
        TEAMPLACEMENT,
        UNITSELECTION,
        PLAY,
        END
    }
    private int roundNumber = 1;
    private int turnNumber = 1;
    public static TurnSystem Instance { get; private set; }
    public Action<Phase> OnPhaseChanged;
    public Action OnTeamChanged;
    public Action OnTeamPlacementCompleted;
    private int currentTeamIndex;
    private int initialTeamIndex;
    private int numberOfTeams;
    private Team currentTeam;
    private Phase currentPhase;

    // ------------ INPUTSYSTEM FUNCTIONS -------------------------------------------------------
    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed){ return; }
        if (currentPhase != Phase.TEAMPLACEMENT){ return; }
        
        HandleUnitPlacement();  
    }

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Team.OnAnyTeamDefeated += Team_OnAnyTeamDefeated;
        numberOfTeams = TeamManager.Instance.GetNumberOfTeams();
        initialTeamIndex = UnityEngine.Random.Range(0, numberOfTeams);
        currentTeamIndex = initialTeamIndex;
        currentTeam = TeamManager.Instance.GetTeam(currentTeamIndex);
        StartUnitPlacementPhase();
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void NextTurn()
    {
        ChangeTeam();
        StartSelectionPhase();
    }
    public void StartUnitPlacementPhase()
    {
        currentPhase = Phase.TEAMPLACEMENT;
        OnPhaseChanged.Invoke(currentPhase);
    }

    private void CycleTeam()
    {
        currentTeamIndex = (currentTeamIndex + 1) % numberOfTeams;
        currentTeam = TeamManager.Instance.GetTeam(currentTeamIndex);
        OnTeamChanged.Invoke();
    }

    public void StartSelectionPhase()
    {
        currentPhase = Phase.UNITSELECTION;
        OnPhaseChanged.Invoke(currentPhase);
    }

    public void StartPlayPhase()
    {
        currentPhase = Phase.PLAY;
        OnPhaseChanged.Invoke(currentPhase);
    }

    internal Phase GetCurrentPhase()
    {
        return currentPhase;
    }

    private void ChangeTeam()
    {
        CycleTeam();
        if (currentTeamIndex == initialTeamIndex)
        {
            roundNumber++;
        }
        turnNumber = (turnNumber % numberOfTeams) + 1;
    }

    private void HandleUnitPlacement()
    {
        List<HexAxial> validPlacements = GetValidPlacements();
        HexAxial mouseHexAxial = LevelGridHex.Instance.GetHexAxial(MouseWorld.GetPosition());
        if (validPlacements.Contains(mouseHexAxial))
        {
            TeamManager.Instance.CreateUnit(currentTeam, mouseHexAxial);
            if (currentTeam.IsFull())
            {
                CycleTeam();
                if (currentTeamIndex == initialTeamIndex)
                {
                    OnTeamPlacementCompleted.Invoke();
                    CycleTeam();
                    StartSelectionPhase();
                }
            }
        }
    }

    // ------------ GETTERS -------------------------------------------------------
    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public int GetRoundNumber()
    {
        return roundNumber;
    }

    public Team GetCurrentTeam()
    {
        return currentTeam;
    }

    public int GetCurrentTeamIndex()
    {
        return currentTeamIndex;
    }

    internal bool IsPlayPhase()
    {
        return currentPhase == Phase.PLAY;
    }

    internal bool IsSelectionPhase()
    {
        return currentPhase == Phase.UNITSELECTION;
    }

    public List<HexAxial> GetValidPlacements()
    {
        List<HexAxial> validPlacements = new();
        List<Unit> teammates = currentTeam.GetUnits();
        List<Unit> enemyUnits = TeamManager.Instance.GetAllEnemyUnits(currentTeam);
        foreach (HexAxial hexAxial in LevelGridHex.Instance.GetAllHexes())
        {
            bool isValid = true;
            if (!Pathfinding.Instance.GetNode(hexAxial).IsWalkable()) { continue; }
            foreach (Unit enemy in enemyUnits)
            {
                if (HexUtilities.AxialDistance(enemy.GetHexAxial(), hexAxial) <= 3)
                {
                    isValid = false;
                    break;
                }
            }
            if (isValid)
            {
                foreach (Unit teammate in teammates)
                {
                    if (HexUtilities.AxialDistance(teammate.GetHexAxial(), hexAxial) > 7)
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            if (isValid)
            {
                validPlacements.Add(hexAxial);
            }
        }
        return validPlacements;
    }

    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void Team_OnAnyTeamDefeated()
    {
        currentPhase = Phase.END;
        OnPhaseChanged.Invoke(currentPhase);
    }
}
