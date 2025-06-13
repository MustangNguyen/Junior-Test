using UnityEngine;
using System.Collections;

public class Creep : BaseEnemy
{
    [Header("Creep Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float creepMoveSpeed = 3f;

    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private bool canAttack = true;
    private Vector2 moveDirection;
    private float currentSpeed;
    private Transform player;
    private float lastAttackTime;
    private bool isAttacking;

    protected override void Awake()
    {
        base.Awake();
        moveSpeed = creepMoveSpeed;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
    }

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
    }

    private void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if player is in detection range
        if (distanceToPlayer <= enemyData.detectionRange)
        {
            // Calculate direction to player
            moveDirection = (player.position - transform.position).normalized;

            // Check if in attack range
            if (distanceToPlayer <= enemyData.attackRange)
            {
                currentSpeed = 0f;
                TryAttack();
            }
            else
            {
                // Move towards player
                currentSpeed = moveSpeed;
            }
        }
        else
        {
            // Stop moving if player is out of range
            moveDirection = Vector2.zero;
            currentSpeed = 0f;
        }

        // Update animator if available
        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
            animator.SetBool("IsAttacking", isAttacking);
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = moveDirection * currentSpeed;
        }
    }

    private void TryAttack()
    {
        if (Time.time >= lastAttackTime + enemyData.attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;

        // Deal damage to player if it has IDamageable component
        IDamageable damageable = player.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(enemyData.attackDamage);
        }

        isAttacking = false;
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
        if (direction != Vector2.zero)
        {
            // Move in 2D space
            transform.position += new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.deltaTime;
            
            // Rotate to face movement direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public override void Stop()
    {
        moveDirection = Vector2.zero;
        currentSpeed = 0f;
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