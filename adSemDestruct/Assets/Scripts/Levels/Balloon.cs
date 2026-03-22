using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static LevelGoal;
public class Balloon : MonoBehaviour, IDamageable, IScoreable
{
    #region IDAMAGEABLE
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
        if (health <= 0)
            Destruct();
    }
    public void Destruct()
    {
        touchObjects.Clear();
        Destroy(this.gameObject);
        GiveScore();
    }
    #endregion
    #region ISCOREABLE
    [Header("Scoreable")]
    [SerializeField] public int score;
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
    [SerializeField] List<GameObject> touchObjects = new List<GameObject>();   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(touchObjects.Count <= 0)
        {
            if (collision.gameObject.GetComponent<Block>() == null) 
            collision.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            touchObjects.Add(collision.gameObject);
        }

        else
        {
            foreach (GameObject obj in touchObjects)
            {
                if(obj != null)
                    obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            }
            Destruct();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (touchObjects.Count <= 0)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            touchObjects.Add(collision.gameObject);
        }

        else
        {
            foreach (GameObject obj in touchObjects)
            {
                if (obj != null)
                    obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            }
            this.GetComponent<CircleCollider2D>().isTrigger = true;
            Destruct();
        }
    }

}
