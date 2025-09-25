using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetSelectionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform targetButtonPrefab;
    [SerializeField] private Transform targetButtonContainerTransform;
    private List<TargetButtonUI> targetButtonUIList;

    // ------------ MONOBEHAVIOR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        targetButtonUIList = new List<TargetButtonUI>();
    }

    private void Start()
    {
        ShootAction.OnWaitingForTarget += ShootAction_OnWaitingForTarget;
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    private void CreateTargetSelectionButtons(ShootAction shootAction, List<ICanBeDamaged> targetList)
    {
        foreach (Transform buttonTransform in targetButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }
        targetButtonUIList.Clear();

        foreach (ICanBeDamaged canBeDamaged in targetList)
        {
            Transform targetButtonTransform = Instantiate(targetButtonPrefab, targetButtonContainerTransform);
            TargetButtonUI targetButtonUI = targetButtonTransform.GetComponent<TargetButtonUI>();
            targetButtonUI.SetCanBeDamaged(shootAction, canBeDamaged);

            targetButtonUIList.Add(targetButtonUI);
        }
    }

    private void EnableSelectionButtons()
    {
        targetButtonContainerTransform.gameObject.SetActive(true);
    }

    private void DisableSelectionButtons()
    {
        targetButtonContainerTransform.gameObject.SetActive(false);
    }

    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void ShootAction_OnWaitingForTarget(object sender, List<ICanBeDamaged> targetList)
    {
        ShootAction shootAction = sender as ShootAction;
        EnableSelectionButtons();
        CreateTargetSelectionButtons(shootAction, targetList);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                DisableSelectionButtons();
                break;
        }
    }
}
