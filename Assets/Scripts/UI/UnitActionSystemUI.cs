using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private Transform masterContainer;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private List<ActionButtonUI> actionButtonUIList;
    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }
    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        TurnSystem.Instance.OnPhaseChanged += TurnSystem_OnPhaseChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        masterContainer.gameObject.SetActive(false);

    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    private void CreateUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }
        actionButtonUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);

            actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void EnableMasterContainer()
    {
        masterContainer.gameObject.SetActive(true);
    }

    private void DisableMasterContainer()
    {
        masterContainer.gameObject.SetActive(false);
    }
    private void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }
    private void UpdateActionPointsVisual()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (selectedUnit == null) { return; }
        actionPointsText.text = "Action Points: " + selectedUnit.GetCurrentActionPoints();
    }

    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPointsVisual();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsVisual();
    }
    
    private void TurnSystem_OnPhaseChanged(TurnSystem.Phase currentPhase)
    {
        switch (currentPhase)
        {
            default:
            case TurnSystem.Phase.UNITSELECTION:
                DisableMasterContainer();
                break;
            case TurnSystem.Phase.PLAY:
                EnableMasterContainer();
                break;
        }
    }
}
