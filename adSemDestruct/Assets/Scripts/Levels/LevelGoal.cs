using DG.Tweening;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

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
        CheckHealth();
        UpdateUI();
    }
    public void Destruct()
    {
        this.gameObject.transform.DOPunchScale(new Vector3(1, 1, 1), 0.25f);
        GameObject dParticle = Instantiate(destroyParticlePF.gameObject, transform.position, Quaternion.identity);
        Destroy(dParticle, .2f);

        GameObject sParticle = Instantiate(scoreParticle, transform.position, Quaternion.identity);
        sParticle.GetComponent<ScoreParticle>().Spawn(score);

        levelGoalDestroyed?.Invoke(this);
        Destroy(this.gameObject, 0.25f);
        GiveScore();
        isDestroyed = true;
    }
    #endregion

    #region ISCOREABLE
    [Header("Scoreable")]
    [SerializeField] int score;
    [SerializeField] GameObject scoreParticle;
    public int Score
    {
        get
        {
            return score;
        }
    }

    public GameObject ScoreParticlePF
    {
        get { return scoreParticle; }
    }

    public void GiveScore()
    {
        ScoreManager.instance.AddScore(score);
    }
    #endregion
    
    bool isDestroyed = false;
    [SerializeField] TextMeshProUGUI healthTF;
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
    }

    private void OnDisable()
    {
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
