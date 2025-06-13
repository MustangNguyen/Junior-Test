using UnityEngine;
using Lean.Pool;

public class Bullet : MonoBehaviour
{
    private AmmoDataSO ammoData;
    private float speed;
    private float lifetime;
    private float damage;
    private int maxBounces;
    private int maxPenetration;
    private Vector2 direction;
    private Rigidbody2D rb;
    private int currentBounces;
    private int currentPenetrations;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(AmmoDataSO data, Vector2 dir)
    {
        ammoData = data;
        speed = data.speed;
        lifetime = data.lifetime;
        damage = data.damage;
        maxBounces = data.maxBounces;
        maxPenetration = data.maxPenetration;
        direction = dir.normalized;
        currentBounces = 0;
        currentPenetrations = 0;

        // Set rotation to match direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Set velocity
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }

        // Start lifetime countdown
        Invoke(nameof(Despawn), lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);
        Vector2 normal = contact.normal;

        // Check if hit obstacle
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (currentBounces < maxBounces)
            {
                // Calculate reflection vector using the law of reflection: R = I - 2(N·I)N
                Vector2 reflection = direction - 2 * Vector2.Dot(normal, direction) * normal;
                direction = reflection.normalized;
                currentBounces++;

                // Update rotation to match new direction
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);

                // Update velocity
                if (rb != null)
                {
                    rb.velocity = direction * speed;
                }

                // Spawn impact effect
                if (ammoData.impactEffect != null)
                {
                    LeanPool.Spawn(ammoData.impactEffect, contact.point, Quaternion.identity);
                }
            }
            else
            {
                Despawn();
            }
        }
        // Check if hit enemy
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Deal damage to enemy
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            // Spawn impact effect
            if (ammoData.impactEffect != null)
            {
                LeanPool.Spawn(ammoData.impactEffect, contact.point, Quaternion.identity);
            }

            // Check if can penetrate
            if (currentPenetrations < maxPenetration)
            {
                currentPenetrations++;
                // Ignore collision with this enemy
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
            }
            else
            {
                Despawn();
            }
        }
        else
        {
            Despawn();
        }
    }

    private void Despawn()
    {
        LeanPool.Despawn(gameObject);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Despawn));
    }
} 