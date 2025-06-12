using UnityEngine;

public interface IMovable
{
    // OnMovement speed property
    float moveSpeed { get; set; }
    
    // Basic OnMovement method
    void OnMove(Vector2 direction);
    
    // Stop OnMovement
    void Stop();

    // Rotate the object towards a direction
    void Rotate(Vector2 direction);
}
