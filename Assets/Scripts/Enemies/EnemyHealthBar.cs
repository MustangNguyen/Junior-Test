using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer healthBarRenderer;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0); // Offset from enemy position
    [SerializeField] private Vector3 scale = new Vector3(2f, 0.3f, 1f); // Initial scale of health bar
    [SerializeField] private float colorChangeSpeed = 2f; // Speed of color transition
    [SerializeField] private float scaleChangeSpeed = 5f; // Speed of scale transition

    private Vector3 initialScale;
    private Transform target;
    private Camera mainCamera;
    private float targetHealthPercentage = 1f;
    private Color targetColor;
    private Vector3 targetScale;

    private void Awake()
    {
        if (healthBarRenderer == null)
        {
            healthBarRenderer = GetComponent<SpriteRenderer>();
        }

        mainCamera = Camera.main;
        initialScale = scale;
        transform.localScale = initialScale;
        targetColor = Color.green;
        targetScale = initialScale;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Billboard effect - always face camera
        if (mainCamera != null)
        {
            transform.forward = mainCamera.transform.forward;
        }

        // Smooth color transition
        healthBarRenderer.color = Color.Lerp(healthBarRenderer.color, targetColor, Time.deltaTime * colorChangeSpeed);

        // Smooth scale transition
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleChangeSpeed);
    }

    public void Initialize(Transform enemyTransform)
    {
        target = enemyTransform;
        transform.SetParent(target);
        transform.localPosition = offset;
        transform.localRotation = Quaternion.identity;
    }

    public void UpdateHealthBar(float healthPercentage)
    {
        targetHealthPercentage = healthPercentage;
        Debug.Log("Health Percentage: " + healthPercentage);
        // Update target scale
        targetScale = initialScale;
        targetScale.x *= healthPercentage;

        // Update target color based on health percentage
        if (healthPercentage > 0.6f)
        {
            targetColor = Color.green;
        }
        else if (healthPercentage > 0.3f)
        {
            targetColor = Color.yellow;
        }
        else
        {
            targetColor = Color.red;
        }
    }
} 