using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum DestructionObjectType
{
    GLASS,
    WOOD,
    STONE,
    NUM_TYPES
}
public class LevelMaterial : CustomPhysics, IDamageable, IScoreable
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
    }
    public void TakeDamage(float damage)
    {
        health -= (int)damage;
        if (damage > 0)
        {
            GameObject newParticles = Instantiate(hitParticlesPF.gameObject, transform.position, Quaternion.identity);
            Destroy(newParticles, 0.2f);
        }

        CheckHealth();
        UpdateUI();

    }
    public void Destruct()
    {
        GameObject newParticle = GameObject.Instantiate(destroyParticlesPF.gameObject, transform.position, Quaternion.identity);
        Destroy(newParticle, .2f);
        levelMaterialDestroyed?.Invoke(this);
        Destroy(this.gameObject);
    }
    #endregion 

    #region ISCOREABLE
    [Header("Scoreable")]
    [SerializeField] int score;
    public int Score
    {
        get { return score; }
    }
    public void GiveScore()
    {
        ScoreManager.instance.AddScore(score);
    }
    #endregion

    [SerializeField] DestructionObjectType type;
    public DestructionObjectType getType() { return type; }

    [Header("References")]
    [SerializeField] TextMeshProUGUI healthTF;
    [SerializeField] ParticleSystem hitParticlesPF;
    [SerializeField] ParticleSystem destroyParticlesPF;

    #region EVENTS
    public delegate void LevelMaterialDestroyed(LevelMaterial theMaterial);
    public static event LevelMaterialDestroyed levelMaterialDestroyed;

    #endregion
    void CheckHealth()
    {
        if (health <= 0)
            Destruct();
    }

    #region EVENTS

    public delegate void MaterialHitLevelGoal(float damage, LevelGoal theGoal);
    public static event MaterialHitLevelGoal materialHitLevelGoal;

    public delegate void MaterialHitLevelMaterial(float damage, LevelMaterial theMaterial);
    public static event MaterialHitLevelMaterial materialHitLevelMaterial;

    private void OnEnable()
    {
        Block.blockHitObject += DamageCheck;
        materialHitLevelMaterial += DamageCheck;
    }

    private void OnDisable()
    {
        Block.blockHitObject -= DamageCheck;
        materialHitLevelMaterial -= DamageCheck;
    }
    #endregion

    void Start()
    {
        base.Start();
        health = maxHealth;
        UpdateUI();
    }

    void DamageCheck(float damage, LevelMaterial theDestruct)
    {
        if (theDestruct == this)
        {
            TakeDamage(damage);
        }
    }

    void UpdateUI()
    {
        healthTF.text = string.Format("{0}", health);
    }
   
}
