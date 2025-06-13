using UnityEngine;
using Lean.Pool;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private GunDataSO gunDataSO;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Camera mainCamera;
    
    [Header("Debug")]
    [SerializeField] private bool showDebug = true;
    [SerializeField] private Color debugLineColor = Color.red;
    
    private int currentAmmo;
    private bool isReloading;
    private float nextFireTime;
    private AudioSource audioSource;
    private float movementPenalty = 1f;
    private GunManager gunManager;
    private KeyBindManager keyBindManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        gunManager = GetComponentInParent<GunManager>();
        if (gunManager == null)
        {
            Debug.LogError("GunManager not found in parent hierarchy!");
        }
        keyBindManager = KeyBindManager.Instance;
        if (keyBindManager == null)
        {
            Debug.LogError("KeyBindManager not found!");
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            Debug.Log("Using main camera: " + (mainCamera != null ? mainCamera.name : "null"));
        }
        InitializeGun(gunDataSO);
    }

    private void Update()
    {
        // Rotate gun to follow mouse
        if (mainCamera != null)
        {
            // Get mouse position in world space
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = mainCamera.transform.position.y - transform.position.y;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

            if (showDebug)
            {
                Debug.Log($"Mouse Screen Position: {Input.mousePosition}");
                Debug.Log($"Mouse World Position: {worldPos}");
                Debug.Log($"Gun Position: {transform.position}");
                Debug.Log($"Camera Position: {mainCamera.transform.position}");
            }

            // Calculate direction to mouse position
            Vector3 direction = worldPos - transform.position;
            
            // Calculate angle for Z-axis rotation
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            if (showDebug)
            {
                Debug.Log($"Direction to Mouse: {direction}");
                Debug.Log($"Rotation Angle: {angle}");
                Debug.DrawRay(transform.position, direction, debugLineColor);
                Debug.DrawLine(transform.position, worldPos, Color.yellow);
            }

            // Apply rotation around Z axis
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Handle shooting
            if (keyBindManager != null && keyBindManager.IsShooting())
            {
                Shoot();
            }
        }
        else
        {
            Debug.LogWarning("Main camera is null!");
        }
    }

    public void InitializeGun(GunDataSO newGunDataSO)
    {
        if (newGunDataSO == null) return;
        
        gunDataSO = newGunDataSO;
        currentAmmo = gunDataSO.maxAmmo;
        
        // Update gun model if needed
        if (gunDataSO.gunModel != null)
        {
            // Remove old model if exists
            foreach (Transform child in transform)
            {
                if (child != muzzlePoint)
                {
                    Destroy(child.gameObject);
                }
            }
            
            // Instantiate new model
            Instantiate(gunDataSO.gunModel, transform);
        }
    }

    public bool CanShoot()
    {
        return !isReloading && currentAmmo > 0 && Time.time >= nextFireTime && 
               gunManager.HasAmmo(gunDataSO.ammoType);
    }

    public void Shoot()
    {
        if (!CanShoot()) return;

        if (!gunManager.UseAmmo(gunDataSO.ammoType))
            return;

        nextFireTime = Time.time + (1f / gunDataSO.fireRate) * movementPenalty;
        currentAmmo--;

        // Spawn bullet
        GameObject bulletObj = LeanPool.Spawn(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(gunDataSO.ammoType, muzzlePoint.forward);
        }

        // Play effects
        if (gunDataSO.muzzleFlash != null)
        {
            gunDataSO.muzzleFlash.Play();
        }

        if (gunDataSO.shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gunDataSO.shootSound);
        }
    }

    public void Reload()
    {
        if (isReloading || currentAmmo == gunDataSO.maxAmmo) return;

        isReloading = true;
        
        if (gunDataSO.reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gunDataSO.reloadSound);
        }

        Invoke(nameof(FinishReload), gunDataSO.reloadTime * movementPenalty);
    }

    private void FinishReload()
    {
        currentAmmo = gunDataSO.maxAmmo;
        isReloading = false;
    }

    public void SetMovementPenalty(float penalty)
    {
        movementPenalty = penalty;
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return gunDataSO.maxAmmo;
    }

    public string GetGunName()
    {
        return gunDataSO.gunName;
    }

    public GunDataSO GetGunData()
    {
        return gunDataSO;
    }

    public int GetTotalAmmo()
    {
        return gunManager.GetAmmoCount(gunDataSO.ammoType);
    }
}
