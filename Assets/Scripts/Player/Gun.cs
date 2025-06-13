using UnityEngine;
using Lean.Pool;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private GunDataSO gunDataSO;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Camera mainCamera;
    
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
        }
        InitializeGun(gunDataSO);
    }

    private void Update()
    {
        if (mainCamera != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = mainCamera.transform.position.y - transform.position.y;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            Vector3 direction = worldPos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            if (keyBindManager != null && keyBindManager.IsShooting())
            {
                Shoot();
            }
        }
    }

    public void InitializeGun(GunDataSO newGunDataSO)
    {
        if (newGunDataSO == null) return;
        
        gunDataSO = newGunDataSO;
        currentAmmo = gunDataSO.maxAmmo;
        
        if (gunDataSO.gunModel != null)
        {
            foreach (Transform child in transform)
            {
                if (child != muzzlePoint)
                {
                    Destroy(child.gameObject);
                }
            }
            
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
        if (!CanShoot())
        {
            return;
        }

        if (!gunManager.UseAmmo(gunDataSO.ammoType))
        {
            return;
        }

        float fireRate = gunDataSO.fireRate;
        float timeBetweenShots = 1f / fireRate;
        nextFireTime = Time.time + timeBetweenShots * movementPenalty;
        currentAmmo--;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.transform.position.y - transform.position.y;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        Vector3 direction = (worldPos - transform.position).normalized;

        if (muzzlePoint == null)
        {
            muzzlePoint = transform;
        }

        GameObject bulletObj = LeanPool.Spawn(bulletPrefab, muzzlePoint.position, Quaternion.identity);
        
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(gunDataSO.ammoType, direction);
        }

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
