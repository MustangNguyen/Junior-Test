using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    private Bullet bullet;
    private BoxCollider2D detectorCollider;

    private void Awake()
    {
        detectorCollider = GetComponent<BoxCollider2D>();
    }

    public void Initialize(Bullet bullet)
    {
        this.bullet = bullet;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Khi phát hiện kẻ địch, bật trigger cho đạn
            bullet.SetTriggerMode(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Khi ra khỏi kẻ địch, tắt trigger cho đạn
            bullet.SetTriggerMode(false);
        }
    }
} 