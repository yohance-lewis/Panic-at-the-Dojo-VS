using System;
using Unity.VisualScripting;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private MeshRenderer meshRenderer;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitButtonUI.OnHover += UnitButtonUI_OnHover;
        TurnSystem.Instance.OnPhaseChanged += TurnSystem_OnPhaseChanged;

        UpdateVisual();
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        UnitButtonUI.OnHover -= UnitButtonUI_OnHover;
        TurnSystem.Instance.OnPhaseChanged -= TurnSystem_OnPhaseChanged;
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    private void UpdateVisual()
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() == unit)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }

    }
    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
    {
        UpdateVisual();
    }

    private void UnitButtonUI_OnHover(Unit unit)
    {
        if (this.unit == unit)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    private void TurnSystem_OnPhaseChanged(TurnSystem.Phase currentPhase)
    {
        switch (currentPhase)
        {
            case TurnSystem.Phase.UNITSELECTION:
                meshRenderer.enabled = false;
                break;
        }
    }
}
