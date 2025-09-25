using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitSelectionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform unitButtonPrefab;
    [SerializeField] private Transform unitButtonContainerTransform;

    private List<UnitButtonUI> unitButtonUIList;

    // ------------ MONOBEHAVIOR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        unitButtonUIList = new List<UnitButtonUI>();
    }

    private void Start()
    {
        TurnSystem.Instance.OnPhaseChanged += TurnSystem_OnPhaseChanged;
        DisableSelectionButtons();
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    private void CreateUnitSelectionButtons()
    {
        foreach (Transform buttonTransform in unitButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }
        unitButtonUIList.Clear();

        Team currentTeam = TurnSystem.Instance.GetCurrentTeam();
        List<Unit> units = currentTeam.GetAliveUnits();

        foreach (Unit unit in units)
        {
            Transform unitButtonTransform = Instantiate(unitButtonPrefab, unitButtonContainerTransform);
            UnitButtonUI unitButtonUI = unitButtonTransform.GetComponent<UnitButtonUI>();
            unitButtonUI.SetUnit(unit);

            unitButtonUIList.Add(unitButtonUI);
        }
    }

    private void EnableSelectionButtons()
    {
        unitButtonContainerTransform.gameObject.SetActive(true);
    }

    private void DisableSelectionButtons()
    {
        unitButtonContainerTransform.gameObject.SetActive(false);
    }

    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void TurnSystem_OnPhaseChanged(TurnSystem.Phase currentPhase)
    {
        switch (currentPhase)
        {
            case TurnSystem.Phase.UNITSELECTION:
                EnableSelectionButtons();
                CreateUnitSelectionButtons();
                break;
            case TurnSystem.Phase.PLAY:
                DisableSelectionButtons();
                break;
        }
    }
}
