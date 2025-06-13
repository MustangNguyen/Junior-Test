using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Game/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public GameObject deathEffect;
    public GameObject hitEffect;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public float attackRange = 2f;
    public float detectionRange = 10f;

    [Header("Visual Settings")]
    public Sprite enemySprite;
    public RuntimeAnimatorController animatorController;
} 