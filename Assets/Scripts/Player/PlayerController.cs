using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float rotationSpeed = 10f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 currentVelocity;
    private float currentSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing!");
            return;
        }

        // Configure Rigidbody2D
        rb.gravityScale = 0f;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void HandleMovement()
    {
        if (KeyBindManager.Instance == null) return;

        moveDirection = KeyBindManager.Instance.GetMovementDirection();
        currentSpeed = KeyBindManager.Instance.IsSprinting() ? sprintSpeed : moveSpeed;
    }

    private void ApplyMovement()
    {
        if (rb == null) return;

        // Calculate target velocity
        Vector2 targetVelocity = moveDirection * currentSpeed;

        // Smoothly interpolate current velocity to target velocity
        currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVelocity,
            (moveDirection.magnitude > 0.1f ? acceleration : deceleration) * Time.fixedDeltaTime
        );

        // Apply velocity
        rb.velocity = currentVelocity;

        // Rotate player to face movement direction if moving
        if (moveDirection.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
} 