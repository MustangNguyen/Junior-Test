using UnityEngine;
using TMPro;

public class FloatingTextSpawner : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private float textDuration = 1f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float randomOffset = 0.5f;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color healColor = Color.green;

    public void SpawnDamageText(float damage, Vector3 position)
    {
        if (floatingTextPrefab == null) return;

        // Add random offset to position
        Vector3 spawnPos = position + new Vector3(
            Random.Range(-randomOffset, randomOffset),
            Random.Range(-randomOffset, randomOffset),
            0
        );

        GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
        TextMeshPro textMesh = textObj.GetComponent<TextMeshPro>();
        
        if (textMesh != null)
        {
            textMesh.text = damage.ToString("0");
            textMesh.color = damageColor;
        }

        // Add floating animation
        FloatingText floatingText = textObj.AddComponent<FloatingText>();
        floatingText.Initialize(floatSpeed, textDuration);
    }

    public void SpawnHealText(float heal, Vector3 position)
    {
        if (floatingTextPrefab == null) return;

        // Add random offset to position
        Vector3 spawnPos = position + new Vector3(
            Random.Range(-randomOffset, randomOffset),
            Random.Range(-randomOffset, randomOffset),
            0
        );

        GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
        TextMeshPro textMesh = textObj.GetComponent<TextMeshPro>();
        
        if (textMesh != null)
        {
            textMesh.text = "+" + heal.ToString("0");
            textMesh.color = healColor;
        }

        // Add floating animation
        FloatingText floatingText = textObj.AddComponent<FloatingText>();
        floatingText.Initialize(floatSpeed, textDuration);
    }
}

public class FloatingText : MonoBehaviour
{
    private float floatSpeed;
    private float duration;
    private float currentTime;
    private Vector3 startPosition;

    public void Initialize(float speed, float duration)
    {
        this.floatSpeed = speed;
        this.duration = duration;
        this.currentTime = 0f;
        this.startPosition = transform.position;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        
        if (currentTime >= duration)
        {
            Destroy(gameObject);
            return;
        }

        // Calculate new position
        float progress = currentTime / duration;
        transform.position = startPosition + Vector3.up * floatSpeed * progress;
        
        // Fade out
        float alpha = 1f - progress;
        TextMeshPro textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            Color color = textMesh.color;
            color.a = alpha;
            textMesh.color = color;
        }
    }
} 