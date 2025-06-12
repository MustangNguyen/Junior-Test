using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing!");
        }
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (KeyBindManager.Instance == null) return;

        moveDirection = KeyBindManager.Instance.GetMovementDirection();
        float currentSpeed = KeyBindManager.Instance.IsSprinting() ? sprintSpeed : moveSpeed;
        
        if (rb != null)
        {
            rb.velocity = moveDirection * currentSpeed;
        }
    }
} 