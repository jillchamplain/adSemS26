using TMPro;
using UnityEngine;

public class LevelGoal : MonoBehaviour, IDestructible
{
    [Header("Data")]
    [SerializeField] int maxHealth;
    [SerializeField] int health;
    [Header("References")]
    [SerializeField] TextMeshProUGUI healthTF;
    void CheckHealth()
    {
        if (health <= 0)
            DestroySelf();
    }

    #region EVENTS
    public delegate void LevelGoalDestroyed(LevelGoal theGoal);
    public static event LevelGoalDestroyed levelGoalDestroyed;

    private void OnEnable()
    {
        LevelMaterial.materialHit += TakeDamage;
        Block.blockHitGoal += TakeDamage;
    }

    private void OnDisable()
    {
        LevelMaterial.materialHit -= TakeDamage;
        Block.blockHitGoal -= TakeDamage;
    }

    #endregion

    private void Start()
    {
        health = maxHealth;
        UpdateUI();
    }
    void DestroySelf()
    {
        levelGoalDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    void TakeDamage(float damage, LevelGoal theGoal)
    {
        if(theGoal == this)
        {
            health -= (int)damage;
            CheckHealth();
           UpdateUI();
        }
    }

    void UpdateUI()
    {
       healthTF.text = string.Format("{0}", health);
    }

    #region IDESTRUCTIBLE
    public void Destruct()
    {
        levelGoalDestroyed?.Invoke(this);
        Destroy(this.gameObject);
    }
    #endregion
}
