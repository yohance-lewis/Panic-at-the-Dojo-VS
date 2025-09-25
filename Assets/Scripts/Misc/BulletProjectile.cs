using System;
using DG.Tweening;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVFXPrefab;
    [SerializeField] private float movementScale = 200f;

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void Fire(Vector3 targetPosition, Action<int> dealDamage, int damageAmount)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        transform.DOMove(targetPosition, distance / movementScale, false).SetEase(Ease.Linear).OnComplete(() => OnBulletHit(targetPosition, dealDamage, damageAmount));
    }

    private void OnBulletHit(Vector3 targetPosition,Action<int> dealDamage, int damageAmount)
    {
        trailRenderer.transform.parent = null;
        Destroy(gameObject);
        dealDamage(damageAmount);
        Instantiate(bulletHitVFXPrefab, targetPosition, Quaternion.identity);
    }
    
}
