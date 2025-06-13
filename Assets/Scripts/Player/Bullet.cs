using UnityEngine;
using Lean.Pool;

public class Bullet : MonoBehaviour
{
    private AmmoDataSO ammoData;
    private Vector3 direction;
    private float speed;
    private float damage;
    private float lifetime;
    private float currentLifetime;

    public void Initialize(AmmoDataSO data, Vector3 dir)
    {
        ammoData = data;
        direction = dir.normalized;
        speed = data.speed;
        damage = data.damage;
        lifetime = data.lifetime;
        currentLifetime = 0f;

        // Reset rotation to match direction
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnEnable()
    {
        currentLifetime = 0f;
    }

    private void Update()
    {
        // Move bullet
        transform.position += direction * speed * Time.deltaTime;

        // Check lifetime
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= lifetime)
        {
            Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if hit something that can take damage
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        // Spawn impact effects
        if (ammoData.impactEffect != null)
        {
            LeanPool.Spawn(ammoData.impactEffect, transform.position, Quaternion.identity);
        }

        if (ammoData.impactDecal != null)
        {
            // Spawn decal on the surface
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -direction, out hit, 0.1f))
            {
                LeanPool.Spawn(ammoData.impactDecal, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        // Play impact sound
        if (ammoData.impactSound != null)
        {
            AudioSource.PlayClipAtPoint(ammoData.impactSound, transform.position);
        }

        Despawn();
    }

    private void Despawn()
    {
        LeanPool.Despawn(gameObject);
    }
} 