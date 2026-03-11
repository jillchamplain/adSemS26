using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelGoal : MonoBehaviour, IDestructible
{
    [Header("Data")]
    [SerializeField] int maxHealth;
    [SerializeField] int health;
    bool isDestroyed = false;
    [Header("References")]
    [SerializeField] TextMeshProUGUI healthTF;
    [SerializeField] ParticleSystem destroyParticlesPF;
    void CheckHealth()
    {
        if (health <= 0 && !isDestroyed)
            Destruct();
    }

    #region EVENTS
    public delegate void LevelGoalDestroyed(LevelGoal theGoal);
    public static event LevelGoalDestroyed levelGoalDestroyed;

    private void OnEnable()
    {
        LevelMaterial.materialHitLevelGoal += TakeDamage;
        Block.blockHitGoal += TakeDamage;
    }

    private void OnDisable()
    {
        LevelMaterial.materialHitLevelGoal -= TakeDamage;
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
        this.gameObject.transform.DOPunchScale(new Vector3(1, 1, 1), 0.5f);
        Instantiate(destroyParticlesPF, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 0.5f);
        levelGoalDestroyed?.Invoke(this);
        isDestroyed = true;
    }
    #endregion
}
