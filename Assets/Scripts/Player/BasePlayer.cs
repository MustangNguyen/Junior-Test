using UnityEngine;
using System;

public class BasePlayer : MonoBehaviour, IDamageable, IPlayerStats
{
    [Header("Basic Stats")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float attackSpeed = 1f;
    [SerializeField] protected float defense = 0f;

    // IPlayerStats Implementation
    public float MoveSpeed => moveSpeed;
    public float AttackDamage => attackDamage;
    public float AttackSpeed => attackSpeed;
    public float Defense => defense;

    // IDamagable Implementation
    public float CurrentHealth { get; protected set; }
    public float MaxHealth { get; protected set; }
    public bool IsDead { get; protected set; }
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;

    protected virtual void Awake()
    {
        MaxHealth = 100f;
        CurrentHealth = MaxHealth;
        IsDead = false;
    }

    public virtual void TakeDamage(float damage)
    {
        if (IsDead) return;

        float actualDamage = Mathf.Max(0, damage - defense);
        CurrentHealth = Mathf.Max(0, CurrentHealth - actualDamage);
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    protected virtual void Die()
    {
        if (IsDead) return;

        IsDead = true;
        OnDeath?.Invoke();
        
        // Disable player components
        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null) rigidbody.isKinematic = true;
    }

    protected virtual void OnDestroy()
    {
        OnHealthChanged = null;
        OnDeath = null;
    }

    public void TakeDamage(int damage)
    {
        throw new NotImplementedException();
    }
}
