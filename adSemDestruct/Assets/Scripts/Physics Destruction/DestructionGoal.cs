using UnityEngine;

public class DestructionGoal : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] int maxHealth;
    [SerializeField] int health;
    void CheckHealth()
    {
        if (health <= 0)
            DestroySelf();
    }

    #region EVENTS
    public delegate void DestructionGoalDestroyed(DestructionGoal theGoal);
    public static event DestructionGoalDestroyed destructionGoalDestroyed;

    private void OnEnable()
    {
        DestructionObject.destructionHit += TakeDamage;
        Block.blockHitGoal += TakeDamage;
    }

    private void OnDisable()
    {
        DestructionObject.destructionHit -= TakeDamage;
        Block.blockHitGoal -= TakeDamage;
    }

    #endregion

    private void Start()
    {
        health = maxHealth;
    }
    void DestroySelf()
    {
        destructionGoalDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    void TakeDamage(int damage, DestructionGoal theGoal)
    {
        if(theGoal == this)
        {
            health -= damage;
            CheckHealth();
        }
    }

}
