using UnityEngine;

[CreateAssetMenu(fileName = "AmmoData", menuName = "Weapons/Ammo Data")]
public class AmmoDataSO : ScriptableObject
{
    [Header("Ammo Settings")]
    public string ammoName;
    public float damage;
    public float speed;
    public float lifetime;
    public int maxBounces = 3;
    public int maxPenetration = 3;

    [Header("Effects")]
    public GameObject impactEffect;
    public GameObject trailEffect;
} 