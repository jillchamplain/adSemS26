using TMPro;
using UnityEngine;

public enum DestructionObjectType
{
    GLASS,
    WOOD,
    STONE,
    NUM_TYPES
}
public class LevelMaterial : Destructor
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

    public delegate void MaterialHit(int damage, LevelGoal theGoal);
    public static event MaterialHit materialHit;

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
        UpdateUI();
    }

    void TakeDamage(int damage, LevelMaterial theDestruct)
    {
        if (theDestruct == this)
        {
            health -= damage;
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
            materialHit?.Invoke(CalcForceDamage(), collision.gameObject.GetComponent<LevelGoal>());
        }
    }

}
