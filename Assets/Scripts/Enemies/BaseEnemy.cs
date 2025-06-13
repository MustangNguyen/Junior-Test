using UnityEngine;
using UnityEngine.Events;

public class BaseEnemy : MonoBehaviour, IMovable, IDamageable
{
    [Header("Enemy Data")]
    [SerializeField] protected EnemyDataSO enemyData;
    [SerializeField] protected EnemyHealthBar healthBar;

    protected float currentHealth;
    protected bool isDead;

    public UnityEvent onDeath;
    public UnityEvent<float> onTakeDamage; // Pass current health percentage

    public float moveSpeed { get; set; }

    protected virtual void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError($"EnemyDataSO is missing on {gameObject.name}!");
            return;
        }

        // Initialize health
        currentHealth = enemyData.maxHealth;
        moveSpeed = enemyData.moveSpeed;

        // Initialize health bar if available
        if (healthBar != null)
        {
            healthBar.Initialize(transform);
            healthBar.UpdateHealthBar(1f); // Full health at start
        }

        // Initialize visual components
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            spriteRenderer.sprite = enemyData.enemySprite;
        }

        if (TryGetComponent<Animator>(out var animator))
        {
            animator.runtimeAnimatorController = enemyData.animatorController;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        // Spawn hit effect
        if (enemyData.hitEffect != null)
        {
            Instantiate(enemyData.hitEffect, transform.position, Quaternion.identity);
        }

        // Update health bar
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth / enemyData.maxHealth);
        }

        // Invoke take damage event with health percentage
        onTakeDamage?.Invoke(currentHealth / enemyData.maxHealth);
        
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
        if (enemyData.deathEffect != null)
        {
            Instantiate(enemyData.deathEffect, transform.position, Quaternion.identity);
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
