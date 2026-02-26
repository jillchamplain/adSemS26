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
    void CheckHealth()
    {
        if(health <= 0)
            Destroy(gameObject);
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
            //Debug.Log("Taking " + damage);
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
        //Debug.Log(this + " is destroyed"); 
        Destroy(this.gameObject);
    }
    #endregion

}
