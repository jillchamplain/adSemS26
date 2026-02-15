using UnityEngine;

public enum DestructibleType
{
    GLASS,
    WOOD,
    STONE,
    NUM_TYPES
}
public class Destructible : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] DestructibleType type;
    public DestructibleType getType() { return type; }
    [SerializeField] int maxHealth;
    int health;
    void CheckHealth()
    {
        if(health <= 0)
            Destroy(gameObject);
    }

    #region EVENT
    private void OnEnable()
    {
        Block.blockHit += TakeDamage;
    }

    private void OnDisable()
    {
        Block.blockHit -= TakeDamage;
    }
    #endregion
    void Start()
    {
        health = maxHealth;
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        CheckHealth();
    }
}
