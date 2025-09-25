using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UIElements.Experimental;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private GameObject UIContainer;

    // ------------ MONOBEHAVIOR FUNCTIONS -------------------------------------------------------
    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionChanged;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        UpdateActionPointsText();
        UpdateHealthBar();
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(unit.GetWorldPosition()));
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, wallLayerMask))
        {
            if (raycastHit.transform.TryGetComponent<Wall>(out Wall wall))
            {
                if (UIContainer.activeSelf && wall.GetMeshRenderer().material.color.a > 0.6)
                {
                    UIContainer.SetActive(false);
                }
                else if( wall.GetMeshRenderer().material.color.a <= 0.6)
                {
                    UIContainer.SetActive(true);
                }
            }
            
        }
        else if (!UIContainer.activeSelf)
        {
            UIContainer.SetActive(true);
        }
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetCurrentActionPoints().ToString();
    }

    private void UpdateHealthBar()
    {
        float swag = healthSystem.GetHealthNormalized();
        if (swag != 0)
        {
            DOTween.To(() => healthBarImage.fillAmount, x => healthBarImage.fillAmount = x, swag, 0.5f).SetEase(Ease.OutSine);
        }
    }

    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void Unit_OnAnyActionChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }
}
