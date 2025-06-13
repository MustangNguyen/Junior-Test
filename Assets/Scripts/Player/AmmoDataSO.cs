using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo Data", menuName = "Gun System/Ammo Data")]
public class AmmoDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string ammoName;
    public GameObject ammoModel;
    
    [Header("Stats")]
    public float damage;
    public float speed;
    public float lifetime;
    
    [Header("Effects")]
    public AudioClip impactSound;
    public ParticleSystem impactEffect;
    public GameObject impactDecal;
} 