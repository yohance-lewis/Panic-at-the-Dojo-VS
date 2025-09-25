using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float totalSpinAmount;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        actionPointCost = 2;
        actionName = "spin";
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        float spinAddAmount = 360f * Time.deltaTime;
        unit.transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        totalSpinAmount += spinAddAmount;
        if (totalSpinAmount >= 360f)
        {
            ActionComplete();
        }

    }

    // ------------ OVERRIDES -------------------------------------------------------
    public override void TakeAction(HexAxial hexAxial, Action onActionComplete)
    {
        ActionStart(onActionComplete);
        totalSpinAmount = 0;
    }

    public override void GenerateValidActionHexAxialList()
    {
        HexAxial unitHexAxial = unit.GetHexAxial();
        validHexAxialList= new List<HexAxial>
        {
            unitHexAxial
        };
    }
}
