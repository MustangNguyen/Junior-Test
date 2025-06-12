using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Data", menuName = "Gun System/Gun Data")]
public class GunDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string gunName;
    public GameObject gunModel;
    
    [Header("Stats")]
    public float damage;
    public float fireRate;
    public int maxAmmo;
    public float reloadTime;
    
    [Header("Effects")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public ParticleSystem muzzleFlash;

    // public GunData ToGunData()
    // {
    //     return new GunData
    //     {
    //         gunName = this.gunName,
    //         gunModel = this.gunModel,
    //         damage = this.damage,
    //         fireRate = this.fireRate,
    //         maxAmmo = this.maxAmmo,
    //         reloadTime = this.reloadTime,
    //         shootSound = this.shootSound,
    //         reloadSound = this.reloadSound,
    //         muzzleFlash = this.muzzleFlash
    //     };
    // }
} 