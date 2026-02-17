using UnityEngine;

public enum DestructionObjectType
{
    GLASS,
    WOOD,
    STONE,
    NUM_TYPES
}
public class DestructionObject : Destructor
{
    [Header("Data")]
    [SerializeField] DestructionObjectType type;
    public DestructionObjectType getType() { return type; }
    [SerializeField] int maxHealth;
    [SerializeField] int health;
    void CheckHealth()
    {
        if(health <= 0)
            Destroy(gameObject);
    }

    #region EVENTS

    public delegate void DestructionHit(int damage, DestructionGoal theGoal);
    public static event DestructionHit destructionHit;

    private void OnEnable()
    {
        Block.blockHitObject += TakeDamage;
    }

    private void OnDisable()
    {
        Block.blockHitObject -= TakeDamage;
    }
    #endregion

    void Start()
    {
        health = maxHealth;
    }

    void TakeDamage(int damage, DestructionObject theDestruct)
    {
        if (theDestruct == this)
        {
            health -= damage;
            Debug.Log("Taking " + damage);
            CheckHealth();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<DestructionGoal>())
        {
            destructionHit?.Invoke(CalcForceDamage(), collision.gameObject.GetComponent<DestructionGoal>());
        }
    }

}
