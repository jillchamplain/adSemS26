using UnityEngine;

public interface IDamageable
{
    [SerializeField] int Health {  get; set; }
    [SerializeField] int MaxHealth {  get; }
    public void TakeDamage(float damage);
    public void Destruct();
}
