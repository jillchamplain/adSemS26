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
public class LevelMaterial : CustomPhysics, IDamageable
{
    [Header("Data")]
    [SerializeField] DestructionObjectType type;
    public DestructionObjectType getType() { return type; }
    [SerializeField] int maxHealth;
    [SerializeField] int health;
    [SerializeField] public int scoreValue;
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
        Block.blockHitObject += TakeDamage;
        materialHitLevelMaterial += TakeDamage;
    }

    private void OnDisable()
    {
        Block.blockHitObject -= TakeDamage;
        materialHitLevelMaterial -= TakeDamage;
    }
    #endregion

    void Start()
    {
        health = maxHealth;
        UpdateUI();
    }

    void TakeDamage(float damage, LevelMaterial theDestruct)
    {
        if (theDestruct == this)
        {
            //Debug.Log(this + " listened");
            //Debug.Log("Took " + damage);
            health -= (int)damage;
            if(damage > 0)
            {
                GameObject newParticles = Instantiate(hitParticlesPF.gameObject, transform.position, Quaternion.identity);
                Destroy(newParticles, 0.2f);
            }

            CheckHealth();
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        healthTF.text = string.Format("{0}", health);
    }
    #region IDESTRUCTIBLE
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
}
