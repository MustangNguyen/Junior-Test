using UnityEngine;
using Lean.Pool;

public class Bullet : MonoBehaviour
{
    private AmmoDataSO ammoData;
    private Vector2 direction;
    private float speed;
    private float damage;
    private float lifetime;
    private float currentLifetime;

    public void Initialize(AmmoDataSO data, Vector3 dir)
    {
        ammoData = data;
        direction = new Vector2(dir.x, dir.y).normalized;
        speed = data.speed;
        damage = data.damage;
        lifetime = data.lifetime;
        currentLifetime = 0f;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnEnable()
    {
        currentLifetime = 0f;
    }

    private void Update()
    {
        transform.position += new Vector3(direction.x, direction.y, 0) * speed * Time.deltaTime;

        currentLifetime += Time.deltaTime;
        if (currentLifetime >= lifetime)
        {
            Despawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        if (ammoData.impactEffect != null)
        {
            LeanPool.Spawn(ammoData.impactEffect, transform.position, Quaternion.identity);
        }

        if (ammoData.impactDecal != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -direction, 0.1f);
            if (hit.collider != null)
            {
                LeanPool.Spawn(ammoData.impactDecal, hit.point, Quaternion.Euler(0, 0, Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg));
            }
        }

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