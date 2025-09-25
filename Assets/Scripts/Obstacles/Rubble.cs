using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Rubble : Obstacle
{
    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    protected override void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        base.OnEnable();
        LevelGridHex.Instance.AddRubbleAtHexAxial(hexAxial, this);
        
    }

    private void OnDestroy()
    {
        LevelGridHex.Instance.RemoveRubbleAtHexAxial(hexAxial);
    }
}
