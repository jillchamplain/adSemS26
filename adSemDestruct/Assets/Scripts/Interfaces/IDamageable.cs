using UnityEngine;

public interface IDamageable
{
    [SerializeField] int Health {  get; set; }
    [SerializeField] int MaxHealth {  get; }

    [SerializeField] GameObject HitParticlePF { get; }
    [SerializeField] GameObject DestroyParticlePF { get;  }
    public void TakeDamage(float damage);
    public void Destruct();
}
