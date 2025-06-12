using UnityEngine;
using System.Collections.Generic;

public class GunManager : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private Gun currentGun;
    [SerializeField] private List<GunDataSO> availableGuns = new List<GunDataSO>();

    [Header("Movement Tracking")]
    [SerializeField] private float movementThreshold = 0.1f;
    [SerializeField] private float accuracyPenaltyMultiplier = 0.5f;

    private Dictionary<string, GunDataSO> gunDatabase = new Dictionary<string, GunDataSO>();
    private CharacterController playerController;
    private Vector3 lastPosition;
    private bool isMoving;

    private void Start()
    {
        // Initialize gun database
        foreach (var gunData in availableGuns)
        {
            if (!gunDatabase.ContainsKey(gunData.gunName))
            {
                gunDatabase.Add(gunData.gunName, gunData);
            }
        }

        // Initialize with first gun if available
        if (availableGuns.Count > 0)
        {
            SwitchGun(availableGuns[0].gunName);
        }

        // Get player controller reference
        playerController = GetComponentInParent<CharacterController>();
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController not found in parent hierarchy!");
        }

        lastPosition = transform.position;
    }

    private void Update()
    {
        TrackMovement();
    }

    private void TrackMovement()
    {
        if (playerController != null)
        {
            // Calculate movement
            Vector3 currentPosition = transform.position;
            float movement = Vector3.Distance(currentPosition, lastPosition);
            
            // Update movement state
            isMoving = movement > movementThreshold;
            
            // Apply movement effects to current gun if needed
            if (currentGun != null)
            {
                currentGun.SetMovementPenalty(isMoving ? accuracyPenaltyMultiplier : 1f);
            }

            lastPosition = currentPosition;
        }
    }

    public void SwitchGun(string gunName)
    {
        if (gunDatabase.TryGetValue(gunName, out GunDataSO gunDataSO))
        {
            currentGun.InitializeGun(gunDataSO);
        }
        else
        {
            Debug.LogWarning($"Gun '{gunName}' not found!");
        }
    }

    public void PickupGun(GunDataSO newGunDataSO)
    {
        // Add to database if it's a new gun type
        if (!gunDatabase.ContainsKey(newGunDataSO.gunName))
        {
            gunDatabase.Add(newGunDataSO.gunName, newGunDataSO);
        }

        // Switch to the new gun
        currentGun.InitializeGun(newGunDataSO);
    }

    public Gun GetCurrentGun()
    {
        return currentGun;
    }

    public List<string> GetAvailableGunNames()
    {
        return new List<string>(gunDatabase.Keys);
    }

    public bool IsPlayerMoving()
    {
        return isMoving;
    }
} 