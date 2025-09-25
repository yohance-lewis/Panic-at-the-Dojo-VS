using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using System.Collections.Generic;
public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    // public event EventHandler OnActionStarted;
    private Unit selectedUnit;
    private BaseAction selectedAction;
    [SerializeField] private LayerMask unitLayerMask;
    private Unit[] unitArray;
    private bool isPointerNotOverGameObject;

    private bool isBusy;

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
        unitArray = FindObjectsByType<Unit>(FindObjectsSortMode.None);
    }
    private void Update()
    {
        isPointerNotOverGameObject = true;
        if (EventSystem.current.IsPointerOverGameObject())
        {
            isPointerNotOverGameObject = false;
        }
    }

    // ------------ INPUTSYSTEM FUNCTIONS -------------------------------------------------------
    public void OnClick(InputAction.CallbackContext context)
    {
        if (isBusy)
        {
            if (context.interaction is TapInteraction && isPointerNotOverGameObject)
            {
            if (!context.started){ return; }
                switch (selectedAction)
                {
                    case IClickableAction clickableAction:
                        clickableAction.OnClick();
                        break;
                }
                
            }
            return;
        }
        if (context.interaction is TapInteraction && isPointerNotOverGameObject)
        {
            if (context.performed) { return; }
            
            switch (TurnSystem.Instance.GetCurrentPhase())
            {
                case TurnSystem.Phase.PLAY:
                    HandleSelectedAction();
                    break;
            }
        }
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    private void HandleSelectedAction()
    {
        HexAxial mouseHexAxial = LevelGridHex.Instance.GetHexAxial(MouseWorld.GetPosition());
        if (selectedAction.IsValidActionHexAxial(mouseHexAxial))
        {
            if (selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
            {
                SetBusy();
                selectedAction.TakeAction(mouseHexAxial, ClearBusy);
            }
        }
    }

    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }
    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }
    
    private void SetBusy()
    {
        isBusy = true;
        GridSystemVisual.Instance.HideAllHexAxials();
        OnBusyChanged?.Invoke(this, isBusy);
    }
    private void ClearBusy()
    {
        isBusy = false;
        selectedUnit.GenerateValidHexlist();
        GridSystemVisual.Instance.UpdateGridVisual();
        OnBusyChanged?.Invoke(this, isBusy);
    }

    // ------------ GETTERS -------------------------------------------------------  
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    public Unit[] GetUnitArray()
    {
        return unitArray;
    }

    public bool IsBusy()
    {
        return isBusy;
    }
}
