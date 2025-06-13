using UnityEngine;
using Lean.Pool;
using System.Collections.Generic;

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
    private HashSet<Collider2D> penetratedEnemies = new HashSet<Collider2D>();
    private BoxCollider2D mainCollider;
    private BulletDetector detector;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<BoxCollider2D>();
        detector = GetComponentInChildren<BulletDetector>();
        
        // Configure Rigidbody2D
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        if (detector != null)
        {
            detector.Initialize(this);
        }
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
        penetratedEnemies.Clear();

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

    public void SetTriggerMode(bool isTrigger)
    {
        if (mainCollider != null)
        {
            mainCollider.isTrigger = isTrigger;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);
        Vector2 normal = contact.normal;

        // Handle obstacle collision
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (currentBounces < maxBounces)
            {
                // Calculate reflection using the law of reflection: R = I - 2(N·I)N
                float dotProduct = Vector2.Dot(normal, direction);
                direction = direction - 2 * dotProduct * normal;
                direction.Normalize();
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handle enemy collision
        if (other.CompareTag("Enemy"))
        {
            // Only process if we haven't penetrated this enemy before
            if (!penetratedEnemies.Contains(other))
            {
                // Deal damage to enemy
                IDamageable damageable = other.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }

                // Spawn impact effect
                if (ammoData.impactEffect != null)
                {
                    LeanPool.Spawn(ammoData.impactEffect, other.transform.position, Quaternion.identity);
                }

                // Add to penetrated enemies list
                penetratedEnemies.Add(other);

                // Check if can penetrate more enemies
                if (currentPenetrations < maxPenetration)
                {
                    currentPenetrations++;
                }
                else
                {
                    Despawn();
                }
            }
        }
    }

    private void Despawn()
    {
        LeanPool.Despawn(gameObject);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Despawn));
        penetratedEnemies.Clear();
    }
} 