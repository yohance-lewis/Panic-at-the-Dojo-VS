using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(HexRenderer))]
public class Wall : Obstacle
{
    [SerializeField] private Transform rubblePrefab;
    private MeshCollider mc;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void Update()
    {
        if(isBusy){ return; }
        if (isTargeted) { return; }

        if (Vector3.Distance(transform.position, Camera.main.gameObject.transform.position) <= 12)
        {
            if (meshRenderer.material.color.a == 1f)
            {
                meshRenderer.material.DOFade(0.35f, 0.2f);
            }
        }
        else
        {
            if (meshRenderer.material.color.a != 1f)
            {
                meshRenderer.material.DOFade(1f, 0.2f);
            }
        }
    }
    protected override void OnEnable()
    {
        HexRenderer hexRenderer = GetComponent<HexRenderer>();
        meshRenderer = hexRenderer.GetMeshRenderer();
        base.OnEnable();
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        LevelGridHex.Instance.AddWallAtHexAxial(hexAxial, this);
        mc = gameObject.AddComponent<MeshCollider>();
        mc.convex = true;
    }

    private void OnDestroy()
    {
        BaseAction.OnAnyActionStarted -= BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted -= BaseAction_OnAnyActionCompleted;
        LevelGridHex.Instance.RemoveWallAtHexAxial(hexAxial);
    }
    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public override void Damage(int damageAmount)
    {
        if (damageAmount >= damageThreshold)
        {
            Destroy(gameObject);
            CreateRubble();
        }
    }

    private void CreateRubble()
    {
        GameObject.Instantiate(rubblePrefab, GetWorldPosition(), Quaternion.identity);
    }
    // ------------ EVENT LISTENERES -------------------------------------------------------
    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                isBusy = true;
                meshRenderer.material.color = defaultColor;
                if ((object)this != shootAction.GetTarget())
                {
                    meshRenderer.material.DOFade(0.2f, 0.2f);
                }
                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                isBusy = false;
                meshRenderer.material.DOFade(1f, 0.2f);
                break;
        }
    }
}
