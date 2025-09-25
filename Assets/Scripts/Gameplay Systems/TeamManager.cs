using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance { get; private set; }
    [SerializeField] private Transform unitPrefab;
    private List<Team> teams;

    // ------------ MONOBEHAVIOR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        teams = new()
        {
            new Team(Color.red),
            new Team(Color.blue)
        };
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void CreateUnit(Team team, HexAxial position)
    {
        Transform newUnitTransform = Instantiate(unitPrefab, LevelGridHex.Instance.GetWorldPosition(position), Quaternion.identity);
        Unit newUnit = newUnitTransform.GetComponent<Unit>();
        newUnit.SetTeam(team);
        team.AddUnit(newUnit);
    }

    // ------------ GETTERS -------------------------------------------------------
    public Team GetTeam(int teamNumber)
    {
        return teams[teamNumber];
    }

    public int GetNumberOfTeams()
    {
        return teams.Count;
    }

    public List<Unit> GetAllEnemyUnits(Team team)
    {
        List<Unit> enemyUnits = new();
        foreach (Team otherTeam in teams)
        {
            if (team == otherTeam) { continue; }
            enemyUnits = enemyUnits.Concat(otherTeam.GetUnits()).ToList();
        }

        return enemyUnits;
    }
}
