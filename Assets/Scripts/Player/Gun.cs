using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private GunDataSO gunDataSO;
    [SerializeField] private Transform muzzlePoint;
    
    private int currentAmmo;
    private bool isReloading;
    private float nextFireTime;
    private AudioSource audioSource;
    private float movementPenalty = 1f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        InitializeGun(gunDataSO);
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
        return !isReloading && currentAmmo > 0 && Time.time >= nextFireTime;
    }

    public void Shoot()
    {
        if (!CanShoot()) return;

        nextFireTime = Time.time + (1f / gunDataSO.fireRate) * movementPenalty;
        currentAmmo--;

        // Play effects
        if (gunDataSO.muzzleFlash != null)
        {
            gunDataSO.muzzleFlash.Play();
        }

        if (gunDataSO.shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gunDataSO.shootSound);
        }

        // TODO: Implement actual shooting logic (raycast, projectile, etc.)
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
}
