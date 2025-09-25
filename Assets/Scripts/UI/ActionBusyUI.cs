using System;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        gameObject.SetActive(false);
    }

    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        gameObject.SetActive(isBusy);
    }
}
