using System.Collections;
using UnityEngine;

public abstract class CustomPhysics : MonoBehaviour
{
    [Header("Custom Physics")]
    [SerializeField] bool shouldPhysics = false;
    [SerializeField] protected Rigidbody2D rb;
    public void setRigidBody(Rigidbody2D rb) { this.rb = rb; }

    [Tooltip("The minimum damage done to an object")]
    [SerializeField] static float damageMin = 1;
    public void setForceDamageMin(float newMin) { damageMin = newMin; }
    [Tooltip("The minimum damage this needs to calculate to register damage")]
    [SerializeField] float forceDamageThreshold;

    [SerializeField] float weightMultiplier = 1;
    public void setWeightMultiplier(float newWeight) { weightMultiplier = newWeight; }
    [Tooltip("Minimum change in linear velocity before this applies an impulse force")]
    [SerializeField] float forceApplyMin;

    [Header("Collision Damage")]
    [SerializeField] protected Vector2 lastVelocity;
    [Tooltip("Linear velocity of last frame")]
    [SerializeField] float minRegisterVelocity;
    [Tooltip("Minimum velocity this needs to reach in order to calculate collision damage")]

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = weightMultiplier * rb.mass;
        lastVelocity = rb.linearVelocity;
    }

    virtual public void Start()
    {
        StartCoroutine(EnablePhysicsAfter(.1f));
    }
    private void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity * Time.deltaTime;
    }

    IEnumerator EnablePhysicsAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);   
        shouldPhysics = true;
    }

    public void ApplyForce(Vector2 force)
    {
        /* Vector2 curVelocity = rb.linearVelocity;
         Vector2 minVelocityTarget = new Vector2(lastVelocity.x + forceApplyMin, lastVelocity.y + forceApplyMin);
         if(minVelocityTarget.x <= curVelocity.x || minVelocityTarget.y <= curVelocity.y)
         {
             //Debug.Log("Min Velocity is " + minVelocityTarget);
             rb.AddForce(curVelocity, ForceMode2D.Impulse);
             //Debug.Log("Applying Force Of" + curVelocity);
         }
         lastVelocity = curVelocity;*/
        rb.AddRelativeForce(force);
    }

    public Vector2 CalcForce(Collision2D collision)
    {
        Vector2 force = Vector2.zero;

        Vector2 normal = collision.contacts[0].normal;
        Vector2 sendingVelocity = collision.relativeVelocity;
        Vector2 reflectedDirection = Vector2.Reflect(sendingVelocity, normal);

        force = reflectedDirection;
        //float forceDamage = forceDamageMult * Mathf.Abs(this.gameObject.GetComponent<Rigidbody2D>().linearVelocityX); //Need Angle of velocity 
        //float mass = rb.mass;
        // Need X and Y
        //Need mass
        //Apply Force
        return force;

    }

    protected float CalcCollisionDamage(Vector2 reachedVelocity)
    {
        float damage = 0;
        float reachedX = Mathf.Abs(reachedVelocity.x);
        float reachedY = Mathf.Abs(reachedVelocity.y);
        if (reachedX >= minRegisterVelocity || reachedY >= minRegisterVelocity)
        {
            //Debug.Log("start health is " + mHealth);
            //Debug.Log(this.gameObject + "fell");
            damage = Mathf.Max(damageMin,reachedX, reachedY);
        }
        return damageMin;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!shouldPhysics)
            return;
        float damage = CalcCollisionDamage(lastVelocity);
        if(GetComponent<IDamageable>() != null)
            GetComponent<IDamageable>().TakeDamage(damage);

        if (collision.gameObject.GetComponent<IDamageable>() != null)
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);

        if(collision.gameObject.GetComponent<CustomPhysics>() != null)
        {
            collision.gameObject.GetComponent<CustomPhysics>().ApplyForce(CalcForce(collision));
        }

    }

}
