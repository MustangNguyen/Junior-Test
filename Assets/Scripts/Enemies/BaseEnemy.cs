using UnityEngine;
using UnityEngine.Events;

public class BaseEnemy : MonoBehaviour, IMovable, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected GameObject deathEffect;
    [SerializeField] protected GameObject hitEffect;

    protected float currentHealth;
    protected bool isDead;

    public UnityEvent onDeath;
    public UnityEvent<float> onTakeDamage; // Pass current health percentage

    public float moveSpeed { get; set; }

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        // Spawn hit effect
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // Invoke take damage event with health percentage
        onTakeDamage?.Invoke(currentHealth / maxHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    protected virtual void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Invoke death event
        onDeath?.Invoke();

        // Destroy the enemy
        Destroy(gameObject);
    }
    
    public virtual void OnMove(Vector2 direction)
    {
        // Implement move logic
    }
    
    public virtual void Stop()
    {
        // Implement stop logic
    }
    
    public virtual void Rotate(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
        }
    }
}
