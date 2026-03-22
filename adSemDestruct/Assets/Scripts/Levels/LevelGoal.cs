using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelGoal : CustomPhysics, IDamageable, IScoreable
{
    #region IDAMAGEABLE
    [Header("Damageable")]
    [SerializeField] int health;
    public int Health
    {
        get { return health; }
        set { health = value; }
    }
    [SerializeField] int maxHealth;
    public int MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }
    public void TakeDamage(float damage)
    {

        health -= (int)damage;
        CheckHealth();
        UpdateUI();
    }
    public void Destruct()
    {
        this.gameObject.transform.DOPunchScale(new Vector3(1, 1, 1), 0.25f);
        Instantiate(destroyParticlesPF, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 0.25f);
        levelGoalDestroyed?.Invoke(this);
        GiveScore();
        isDestroyed = true;
    }
    #endregion

    #region ISCOREABLE
    [Header("Scoreable")]
    [SerializeField] int score;
    public int Score
    {
        get
        {
            return score;
        }
    }
    public void GiveScore()
    {
        ScoreManager.instance.AddScore(score);
    }
    #endregion
    
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
        LevelMaterial.materialHitLevelGoal += DamageCheck;
        Block.blockHitGoal += DamageCheck;
    }

    private void OnDisable()
    {
        LevelMaterial.materialHitLevelGoal -= DamageCheck;
        Block.blockHitGoal -= DamageCheck;
    }

    #endregion

    private void Start()
    {
        base.Start();
        health = maxHealth;
        UpdateUI();
    }

    void DamageCheck(float damage, LevelGoal theGoal)
    {
        if(theGoal == this)
        {
            TakeDamage(damage);
        }
    }


    void UpdateUI()
    {
       healthTF.text = string.Format("{0}", health);
    }
}
