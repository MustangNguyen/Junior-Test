using UnityEngine;

public class KeyBindManager : MonoBehaviour
{
    public static KeyBindManager Instance { get; private set; }

    [Header("Player Movement Keys")]
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode jump = KeyCode.Space;
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode crouch = KeyCode.C;

    [Header("Gun Control Keys")]
    public KeyCode shoot = KeyCode.Mouse0;
    public KeyCode aim = KeyCode.Mouse1;
    public KeyCode reload = KeyCode.R;
    public KeyCode switchWeapon = KeyCode.Q;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to check if a movement key is pressed
    public bool IsMovementKeyPressed()
    {
        return Input.GetKey(moveUp) || 
               Input.GetKey(moveDown) || 
               Input.GetKey(moveLeft) || 
               Input.GetKey(moveRight);
    }

    // Method to get movement direction
    public Vector3 GetMovementDirection()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(moveUp)) direction += Vector3.up;
        if (Input.GetKey(moveDown)) direction += Vector3.down;
        if (Input.GetKey(moveLeft)) direction += Vector3.left;
        if (Input.GetKey(moveRight)) direction += Vector3.right;

        return direction.normalized;
    }

    // Method to check if shooting
    public bool IsShooting()
    {
        return Input.GetKey(shoot);
    }

    // Method to check if aiming
    public bool IsAiming()
    {
        return Input.GetKey(aim);
    }

    // Method to check if reloading
    public bool IsReloading()
    {
        return Input.GetKeyDown(reload);
    }

    // Method to check if switching weapon
    public bool IsSwitchingWeapon()
    {
        return Input.GetKeyDown(switchWeapon);
    }

    // Method to check if jumping
    public bool IsJumping()
    {
        return Input.GetKeyDown(jump);
    }

    // Method to check if sprinting
    public bool IsSprinting()
    {
        return Input.GetKey(sprint);
    }

    // Method to check if crouching
    public bool IsCrouching()
    {
        return Input.GetKey(crouch);
    }
}
