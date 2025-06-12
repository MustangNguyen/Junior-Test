using UnityEngine;
public interface IDamageable 
{
    void TakeDamage(int damage);
}
public interface IObstacle
{
    void OnInteract();
}