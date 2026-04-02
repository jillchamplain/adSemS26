using DG.Tweening;
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

    [SerializeField] GameObject hitParticlePF;
    public GameObject HitParticlePF
    {
        get { return hitParticlePF; }
    }
    [SerializeField] GameObject destroyParticlePF;
    public GameObject DestroyParticlePF
    {
        get { return destroyParticlePF; }
    }
    public void TakeDamage(float damage)
    {
        health -= (int)damage;
        if (damage > 0)
        {
            GameObject hParticle = Instantiate(hitParticlePF.gameObject, transform.position, Quaternion.identity);
            Destroy(hParticle, 0.2f);
        }

        CheckHealth();
        UpdateUI();

    }
    public void Destruct()
    {
        GiveScore();
        GameObject dParticle = GameObject.Instantiate(destroyParticlePF.gameObject, transform.position, Quaternion.identity);
        Destroy(dParticle, .2f);

        GameObject sParticle = GameObject.Instantiate(scoreParticlePF, transform.position, Quaternion.identity);
        sParticle.GetComponent<TextMeshPro>().DOFade(0f, .5f);
        sParticle.transform.DOLocalMoveY(.1f, 5f);
        Destroy(sParticle, .5f);

        levelMaterialDestroyed?.Invoke(this);
        Destroy(this.gameObject);
    }
    #endregion 

    #region ISCOREABLE
    [Header("Scoreable")]
    [SerializeField] int score;
    [SerializeField] GameObject scoreParticlePF;
    public int Score
    {
        get { return score; }
    }

    public GameObject ScoreParticlePF
    {
        get { return scoreParticlePF; }
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


    private void OnEnable()
    {
        Block.blockHitObject += DamageCheck;
    }

    private void OnDisable()
    {
        Block.blockHitObject -= DamageCheck;
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
