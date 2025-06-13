using UnityEngine;
public interface IDamageable 
{
    void TakeDamage(float damage);
}
public interface IObstacle
{
    void OnInteract();
}