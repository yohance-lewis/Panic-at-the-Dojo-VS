using System;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform ragdollPrefab;
    [SerializeField] private Transform originalRootBone;
    private Color unitColor;
    private HealthSystem healthSystem;

    // ------------ MONOBEHAVIOR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {

        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        unitColor = GetComponent<Unit>().GetColor();
        healthSystem.OnDeath += HealthSystem_OnDeath;
    }

    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void HealthSystem_OnDeath(object sender, EventArgs e)
    {
        Transform ragdollTransform = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>();
        unitRagdoll.Setup(originalRootBone, unitColor);
    }


}
