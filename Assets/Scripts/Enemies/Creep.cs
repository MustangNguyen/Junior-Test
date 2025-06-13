using UnityEngine;
using System.Collections;

public class Creep : BaseEnemy
{
    [Header("Creep Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private bool canAttack = true;
    private Vector2 moveDirection;
    private float currentSpeed;

    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Configure Rigidbody2D
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.drag = 0f;
            rb.angularDrag = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        // Find player as target
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // Check if target is in detection range
        if (distanceToTarget <= detectionRange)
        {
            // Calculate direction to target
            moveDirection = (target.position - transform.position).normalized;

            // Rotate towards target
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Check if in attack range
            if (distanceToTarget <= attackRange && canAttack)
            {
                Attack();
            }
            else
            {
                // Move towards target
                currentSpeed = moveSpeed;
            }
        }
        else
        {
            // Stop moving if target is out of range
            moveDirection = Vector2.zero;
            currentSpeed = 0f;
        }

        // Update animator if available
        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
            animator.SetBool("IsAttacking", !canAttack);
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = moveDirection * currentSpeed;
        }
    }

    private void Attack()
    {
        if (!canAttack) return;

        canAttack = false;
        currentSpeed = 0f;

        // Deal damage to target if it has IDamageable component
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(attackDamage);
        }

        // Start attack cooldown
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public override void OnMove(Vector2 direction)
    {
        moveDirection = direction;
        currentSpeed = moveSpeed;
    }

    public override void Stop()
    {
        moveDirection = Vector2.zero;
        currentSpeed = 0f;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public override void Rotate(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
} 