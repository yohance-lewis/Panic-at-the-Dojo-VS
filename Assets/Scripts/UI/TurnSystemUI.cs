using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class TurnSystemUI : MonoBehaviour

{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private TextMeshProUGUI currentTeamText;
    [SerializeField] private TextMeshProUGUI currentPhaseText;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private GameObject enemyTurnVisualGameObject;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        TurnSystem.Instance.OnPhaseChanged += TurnSystem_OnSelectionPhaseStarted;
        TurnSystem.Instance.OnTeamChanged += TurnSystem_OnTeamChanged;
        TurnSystem.Instance.OnTeamPlacementCompleted += TurnSystem_OnTeamPlacementCompleted;
        UpdateCurrentTeamText();
        endTurnButton.onClick.AddListener(() => { TurnSystem.Instance.NextTurn(); });
        turnNumberText.gameObject.SetActive(false);
        endTurnButton.gameObject.SetActive(false);
        winnerText.gameObject.SetActive(false);
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    private void UpdateTurnText()
    {
        turnNumberText.text = "ROUND " + TurnSystem.Instance.GetRoundNumber() + ", TURN " + TurnSystem.Instance.GetTurnNumber();
    }

    private void UpdateCurrentTeamText()
    {
        currentTeamText.text = "CURRENT TEAM: Team " + (TurnSystem.Instance.GetCurrentTeamIndex() + 1);
    }

    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void TurnSystem_OnSelectionPhaseStarted(TurnSystem.Phase currentPhase)
    {
        switch (currentPhase)
        {
            case TurnSystem.Phase.UNITSELECTION:
                UpdateTurnText();
                break;
            case TurnSystem.Phase.END:
                currentTeamText.gameObject.SetActive(false); 
                turnNumberText.gameObject.SetActive(false);
                endTurnButton.gameObject.SetActive(false);
                winnerText.gameObject.SetActive(true);
                winnerText.text = "TEAM " + (TurnSystem.Instance.GetCurrentTeamIndex() + 1) + " WINS!";
                winnerText.color = TurnSystem.Instance.GetCurrentTeam().GetTeamColor();
                break;
        }
        endTurnButton.gameObject.SetActive(currentPhase == TurnSystem.Phase.PLAY);

    }

    private void TurnSystem_OnTeamChanged()
    {
        UpdateCurrentTeamText();
    }

    private void TurnSystem_OnTeamPlacementCompleted()
    {
        turnNumberText.gameObject.SetActive(true);
        currentPhaseText.gameObject.SetActive(false);
    }
    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        if(TurnSystem.Instance.GetCurrentPhase() == TurnSystem.Phase.END){ return; }
        endTurnButton.gameObject.SetActive(!isBusy);
    }
}
