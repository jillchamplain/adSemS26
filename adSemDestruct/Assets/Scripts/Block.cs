using DG.Tweening;
using System.Security.Cryptography;
using UnityEngine;
using System.Collections.Generic;
public class Block : Destructor
{
    [Header("Data")]
    [SerializeField] MatchShapeType shape;
    public MatchShapeType getMatchShapeType() {  return shape; }
    [SerializeField] MatchItemType type;
    public MatchItemType getMatchItemType() { return type; }
    public void setMatchItemType(MatchItemType newType) {  type = newType; }
    [Header("References")]
    [SerializeField] SpriteRenderer[] spriteRenderers;

    #region EVENTS
    public delegate void BlockCreated();
    public static event BlockCreated blockCreated;

    public delegate void BlockHitObject(int forceDamage, LevelMaterial theDestruct);
    public static event BlockHitObject blockHitObject;

    public delegate void BlockHitGoal(int forceDamage, LevelGoal theDestruct);
    public static event BlockHitGoal blockHitGoal;

    #endregion
    private void Awake()
    {
        
    }
    void Start()
    {
        blockCreated?.Invoke();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DestroySelf()
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.transform.DOPunchScale(new Vector3(.25f, .25f, 0), 0.5f);
        }
        Destroy(gameObject, 0.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<LevelMaterial>())
        {
            blockHitObject?.Invoke(CalcForceDamage(), collision.gameObject.GetComponent<LevelMaterial>());

            DestroySelf();
        }
        else if(collision.gameObject.GetComponent<LevelGoal>())
        {
            blockHitGoal?.Invoke(CalcForceDamage(), collision.gameObject.GetComponent<LevelGoal>());

            DestroySelf();
        }
    }
}
