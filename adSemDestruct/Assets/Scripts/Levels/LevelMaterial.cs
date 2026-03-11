using TMPro;
using UnityEngine;

public enum DestructionObjectType
{
    GLASS,
    WOOD,
    STONE,
    NUM_TYPES
}
public class LevelMaterial : Destructor, IDestructible
{
    [Header("Data")]
    [SerializeField] DestructionObjectType type;
    public DestructionObjectType getType() { return type; }
    [SerializeField] int maxHealth;
    [SerializeField] int health;
    [Header("References")]
    [SerializeField] TextMeshProUGUI healthTF;
    [SerializeField] ParticleSystem hitParticlesPF;
    [SerializeField] ParticleSystem destroyParticlesPF;
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
                Instantiate(hitParticlesPF, transform.position, Quaternion.identity);
            }

            CheckHealth();
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        healthTF.text = string.Format("{0}", health);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.gameObject.GetComponent<LevelGoal>())
        {
            
            materialHitLevelGoal?.Invoke(CalcForceDamage(collision), collision.gameObject.GetComponent<LevelGoal>());
        }
        if(collision.gameObject.GetComponent<LevelMaterial>())
        {
            materialHitLevelMaterial?.Invoke(CalcForceDamage(collision), collision.gameObject.GetComponent <LevelMaterial>());
        }
    }

    #region IDESTRUCTIBLE
    public void Destruct()
    {
        Instantiate(destroyParticlesPF, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
    #endregion

}
