using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDeath;
    public event EventHandler OnDamaged;
    [SerializeField] private int health;
    private int healthMaximum = 20;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        health = healthMaximum;
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void Damage(int damageAmount)
    {
        health = Math.Max(0, health - damageAmount);

        OnDamaged?.Invoke(this, EventArgs.Empty);
        if (health == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    // ------------ GETTERS -------------------------------------------------------
    public float GetHealthNormalized()
    {
        return (float)health / healthMaximum;
    }
}
