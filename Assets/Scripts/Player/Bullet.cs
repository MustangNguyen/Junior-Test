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
    private Rigidbody2D rb;
    private int currentBounces = 0;

    public void Initialize(AmmoDataSO data, Vector3 dir)
    {
        ammoData = data;
        direction = new Vector2(dir.x, dir.y).normalized;
        speed = data.speed;
        damage = data.damage;
        lifetime = data.lifetime;
        currentLifetime = 0f;
        currentBounces = 0;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }

    private void OnEnable()
    {
        currentLifetime = 0f;
        currentBounces = 0;
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }

    private void Update()
    {
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= lifetime)
        {
            Despawn();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HandleObstacleCollision(collision);
            return;
        }

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
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
            ContactPoint2D contact = collision.GetContact(0);
            LeanPool.Spawn(ammoData.impactDecal, contact.point, Quaternion.Euler(0, 0, Mathf.Atan2(contact.normal.y, contact.normal.x) * Mathf.Rad2Deg));
        }

        if (ammoData.impactSound != null)
        {
            AudioSource.PlayClipAtPoint(ammoData.impactSound, transform.position);
        }

        Despawn();
    }

    private void HandleObstacleCollision(Collision2D collision)
    {
        if (currentBounces >= ammoData.maxBounces)
        {
            Despawn();
            return;
        }

        // Get the normal from the collision
        ContactPoint2D contact = collision.GetContact(0);
        Vector2 normal = contact.normal;
        
        // Calculate reflection using the surface normal
        float dotProduct = Vector2.Dot(normal, direction);
        direction = direction - 2 * dotProduct * normal;
        direction.Normalize();
        
        // Update rotation to match new direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Apply new velocity
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }

        // Spawn impact effects
        if (ammoData.impactEffect != null)
        {
            LeanPool.Spawn(ammoData.impactEffect, contact.point, Quaternion.identity);
        }

        if (ammoData.impactDecal != null)
        {
            LeanPool.Spawn(ammoData.impactDecal, contact.point, Quaternion.Euler(0, 0, Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg));
        }

        if (ammoData.impactSound != null)
        {
            AudioSource.PlayClipAtPoint(ammoData.impactSound, contact.point);
        }

        currentBounces++;
    }

    private void Despawn()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        LeanPool.Despawn(gameObject);
    }
} 