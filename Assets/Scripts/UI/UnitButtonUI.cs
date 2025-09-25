using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UnitButtonUI : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    public static Action<Unit> OnHover;

    private Unit unit;
    
    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        textMeshPro.text = unit.ToString();

        button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedUnit(unit);
            TurnSystem.Instance.StartPlayPhase();
        });

    }

    // ------------ INTERFACE IMPLEMENTATION -------------------------------------------------------
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover?.Invoke(unit);
    }
}
