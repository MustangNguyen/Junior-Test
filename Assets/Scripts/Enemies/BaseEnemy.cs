using UnityEngine;

public class BaseEnemy : MonoBehaviour, IMovable
{
    public float moveSpeed { get; set; }
    
    public virtual void OnMove(Vector2 direction)
    {

    }
    
    public virtual void Stop()
    {
        // Implement stop logic
    }
    
    public virtual void Rotate(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
        }
    }

}
