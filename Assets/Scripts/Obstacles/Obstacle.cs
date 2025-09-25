using UnityEngine;
using DG.Tweening;
using System;

public abstract class Obstacle : MonoBehaviour, ICanBeDamaged
{
    protected HexAxial hexAxial;
    protected bool isTargeted = false;
    protected Color defaultColor;
    [SerializeField] protected Color targetedColor;
    protected MeshRenderer meshRenderer;
    protected int damageThreshold = 2;
    protected bool isBusy = false;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    protected virtual void OnEnable()
    {
        hexAxial = LevelGridHex.Instance.GetHexAxial(transform.position);
        defaultColor = meshRenderer.material.color;
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public virtual void Damage(int damageAmount)
    {
        if (damageAmount >= damageThreshold)
        {
            Destroy(gameObject);
        }
    }

    public void SetGlow(bool v)
    {
        if (v)
        {
            isTargeted = true;
            meshRenderer.material.EnableKeyword("_EMISSION");
            meshRenderer.material.DOColor(targetedColor, 0.2f);
        }
        else
        {
            isTargeted = false;
            meshRenderer.material.DisableKeyword("_EMISSION");
            meshRenderer.material.DOColor(defaultColor, 0.2f);
        }
    }

    // ------------ GETTERS FUNCTIONS -------------------------------------------------------
    public HexAxial GetHexAxial()
    {
        return hexAxial;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public MeshRenderer GetMeshRenderer()
    {
        return meshRenderer;
    }
}
