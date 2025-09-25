using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class TargetButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    private ICanBeDamaged canBeDamaged;
    private ShootAction shootAction;
    
    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void SetCanBeDamaged(ShootAction shootAction, ICanBeDamaged canBeDamaged)
    {
        this.canBeDamaged = canBeDamaged;
        this.shootAction = shootAction;
        textMeshPro.text = canBeDamaged.ToString();

        button.onClick.AddListener(() =>
        {
            shootAction.StartShoot(canBeDamaged);
        });

    }
}
