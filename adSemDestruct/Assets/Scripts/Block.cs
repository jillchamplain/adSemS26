using DG.Tweening;
using System.Security.Cryptography;
using UnityEngine;
using System.Collections.Generic;
public class Block : CustomPhysics, IDamageable
{
    [Header("Data")]
    [SerializeField] MatchShapeType shape;
    public MatchShapeType getMatchShapeType() {  return shape; }
    public void setMatchShapeType(MatchShapeType matchShapeType) { shape = matchShapeType; }
    [SerializeField] MatchItemType type;
    public MatchItemType getMatchItemType() { return type; }
    public void setMatchItemType(MatchItemType newType)
    {  type = newType; 
    }
    [SerializeField] Vector2 initialVelocity;
    public Vector2 getInitialVelocity() { return initialVelocity; }
    public void setInitialVelocity(Vector2 newVelocity) {  initialVelocity = newVelocity; }

    [Header("References")]
    [SerializeField] List<SpriteRenderer> spriteRenderers;
    public List<SpriteRenderer> getSpriteRenderers() { return spriteRenderers; }
    public void setSpritesTo(Sprite newSprite)
    {
        foreach(SpriteRenderer spriteR in spriteRenderers)
        {
            spriteR.sprite = newSprite;
        }
    }

    #region EVENTS

    public delegate void BlockHitObject(float forceDamage, LevelMaterial theDestruct);
    public static event BlockHitObject blockHitObject;

    public delegate void BlockHitGoal(float forceDamage, LevelGoal theDestruct);
    public static event BlockHitGoal blockHitGoal;

    #endregion

    private void Start()
    {
        rb.linearVelocity = initialVelocity;
    }
    void DestroySelf()
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.transform.DOPunchScale(new Vector3(.25f, .25f, 0), 0.5f);
        }
        Destroy(gameObject, 0.75f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.GetComponent<LevelMaterial>())
        {
            blockHitObject?.Invoke(CalcForceDamage(collision), collision.gameObject.GetComponent<LevelMaterial>());

            DestroySelf();
        }
        else if(collision.gameObject.GetComponent<LevelGoal>())
        {
            blockHitGoal?.Invoke(CalcForceDamage(collision), collision.gameObject.GetComponent<LevelGoal>());

            DestroySelf();
        }
    }

    #region IDAMAGEABLE
    public void TakeDamage(float damage)
    {

    }
    public void Destruct()
    {
        Destroy(this.gameObject);
    }
    #endregion
}
